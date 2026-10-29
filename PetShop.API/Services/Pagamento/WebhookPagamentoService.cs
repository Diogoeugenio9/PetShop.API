using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Services.Agenda;
using PetShop.API.Utils;

namespace PetShop.API.Services.Pagamento
{
    public class WebhookPagamentoService : IWebhookPagamentoService
    {
        private readonly AppDbContext _context;
        private readonly IAbacatePayClient _abacatePay;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhookPagamentoService> _logger;

        public WebhookPagamentoService(AppDbContext context, IAbacatePayClient abacatePay, IConfiguration configuration, ILogger<WebhookPagamentoService> logger)
        {
            _context = context;
            _abacatePay = abacatePay;
            _configuration = configuration;
            _logger = logger;
        }

        public bool OrigemValida(string corpo, string assinatura, string segredo)
        {
            var segredoEsperado = _configuration["AbacatePay:WebhookSecret"];
            var chavePublica = _configuration["AbacatePay:WebhookChavePublica"];

            if (string.IsNullOrWhiteSpace(segredoEsperado) || string.IsNullOrWhiteSpace(chavePublica))
            {
                _logger.LogError("Webhook do AbacatePay recebido sem WebhookSecret ou WebhookChavePublica configurados.");
                return false;
            }

            if (string.IsNullOrEmpty(segredo) || string.IsNullOrEmpty(assinatura) || corpo == null)
                return false;

            if (!IguaisEmTempoConstante(segredo, segredoEsperado))
                return false;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(chavePublica));
            var calculada = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(corpo)));

            return IguaisEmTempoConstante(calculada, assinatura);
        }

        public async Task Processar(string corpo)
        {
            using var documento = JsonDocument.Parse(corpo);
            var raiz = documento.RootElement;

            var eventoId = Texto(raiz, "id");
            var evento = Texto(raiz, "event");
            if (string.IsNullOrWhiteSpace(eventoId))
                throw new RegraDeNegocioException("Evento sem identificador.");

            var jaProcessado = await _context.EventosPagamento.AnyAsync(e => e.EventoId == eventoId);
            if (jaProcessado)
                return;

            raiz.TryGetProperty("data", out var data);

            switch (evento)
            {
                case "checkout.completed":
                    await ConfirmarCheckout(Texto(data, "checkout", "id") ?? Texto(data, "id"));
                    break;
                case "subscription.completed":
                    await ConfirmarMensal(Texto(data, "subscription", "checkoutId") ?? Texto(data, "checkout", "id"));
                    break;
                case "subscription.renewed":
                    await Renovar(Texto(data, "subscription", "id"));
                    break;
                case "subscription.cancelled":
                    await MarcarCancelada(Texto(data, "subscription", "id"));
                    break;
                default:
                    _logger.LogInformation("Evento {Evento} do AbacatePay ignorado.", evento);
                    break;
            }

            _context.EventosPagamento.Add(new EventoPagamentoModel
            {
                EventoId = eventoId,
                Evento = evento,
                RecebidoEm = DataHoraBrasil.Agora
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ConclusaoAgendamentoService.EhViolacaoDeUnicidade(ex))
            {
                _logger.LogInformation("Evento {EventoId} já havia sido processado por outra requisição.", eventoId);
            }
        }

        private async Task ConfirmarCheckout(string checkoutId)
        {
            var assinatura = await BuscarPorCheckout(checkoutId);
            if (assinatura == null)
                return;

            if (assinatura.Plano == PlanoAssinatura.Mensal)
            {
                await ConfirmarMensal(checkoutId);
                return;
            }

            if (assinatura.Status != StatusAssinatura.Pendente)
                return;

            var checkout = await _abacatePay.BuscarCheckout(checkoutId);
            if (checkout?.Status != "PAID" || checkout.ValorCentavos != EmCentavos(PlanoAssinatura.ValorVitalicio))
            {
                _logger.LogWarning("Checkout {CheckoutId} não confirmado no AbacatePay (status {Status}, valor {Valor}).",
                    checkoutId, checkout?.Status, checkout?.ValorCentavos);
                return;
            }

            var agora = DataHoraBrasil.Agora;
            assinatura.Status = StatusAssinatura.Ativa;
            assinatura.AtivadaEm = agora;
            assinatura.PagoAte = null;

            await EncerrarMensaisDaLoja(assinatura.AdministradorId, agora);
        }

        private async Task ConfirmarMensal(string checkoutId)
        {
            var assinatura = await BuscarPorCheckout(checkoutId);
            if (assinatura == null || assinatura.Plano != PlanoAssinatura.Mensal || assinatura.Status != StatusAssinatura.Pendente)
                return;

            var gateway = await _abacatePay.BuscarAssinaturaDoCheckout(checkoutId);
            if (gateway?.Status != "ACTIVE" || gateway.ValorCentavos != EmCentavos(PlanoAssinatura.ValorMensal))
            {
                _logger.LogWarning("Assinatura do checkout {CheckoutId} não confirmada no AbacatePay (status {Status}, valor {Valor}).",
                    checkoutId, gateway?.Status, gateway?.ValorCentavos);
                return;
            }

            var agora = DataHoraBrasil.Agora;
            assinatura.Status = StatusAssinatura.Ativa;
            assinatura.AssinaturaExternaId = gateway.Id;
            assinatura.AtivadaEm = agora;
            assinatura.PagoAte = agora.AddMonths(1);

            var temVitalicio = await _context.Assinaturas
                .IgnoreQueryFilters()
                .AnyAsync(a => a.AdministradorId == assinatura.AdministradorId
                               && a.Plano == PlanoAssinatura.Vitalicio
                               && a.Status == StatusAssinatura.Ativa);

            if (temVitalicio)
                await EncerrarMensaisDaLoja(assinatura.AdministradorId, agora, assinatura);
        }

        private async Task Renovar(string assinaturaExternaId)
        {
            var assinatura = await BuscarPorAssinaturaExterna(assinaturaExternaId);
            if (assinatura == null || assinatura.Status != StatusAssinatura.Ativa)
                return;

            var agora = DataHoraBrasil.Agora;
            var inicio = assinatura.PagoAte > agora ? assinatura.PagoAte.Value : agora;
            assinatura.PagoAte = inicio.AddMonths(1);
        }

        private async Task MarcarCancelada(string assinaturaExternaId)
        {
            var assinatura = await BuscarPorAssinaturaExterna(assinaturaExternaId);
            if (assinatura == null || assinatura.CanceladaEm.HasValue)
                return;

            assinatura.RenovacaoCancelada = true;
            assinatura.CanceladaEm = DataHoraBrasil.Agora;
        }

        private async Task EncerrarMensaisDaLoja(int administradorId, DateTime agora, AssinaturaModel incluir = null)
        {
            var mensais = await _context.Assinaturas
                .IgnoreQueryFilters()
                .Where(a => a.AdministradorId == administradorId
                            && a.Plano == PlanoAssinatura.Mensal
                            && a.Status == StatusAssinatura.Ativa)
                .ToListAsync();

            if (incluir != null && !mensais.Contains(incluir))
                mensais.Add(incluir);

            foreach (var mensal in mensais)
            {
                if (!string.IsNullOrWhiteSpace(mensal.AssinaturaExternaId) && !mensal.CanceladaEm.HasValue)
                {
                    try
                    {
                        await _abacatePay.CancelarAssinatura(mensal.AssinaturaExternaId);
                    }
                    catch (PagamentoIndisponivelException ex)
                    {
                        _logger.LogError(ex, "Falha ao cancelar a assinatura mensal {AssinaturaId} após a compra vitalícia.", mensal.AssinaturaExternaId);
                        throw;
                    }
                }

                mensal.Status = StatusAssinatura.Cancelada;
                mensal.RenovacaoCancelada = true;
                mensal.CanceladaEm ??= agora;
            }
        }

        private Task<AssinaturaModel> BuscarPorCheckout(string checkoutId)
            => string.IsNullOrWhiteSpace(checkoutId)
                ? Task.FromResult<AssinaturaModel>(null)
                : _context.Assinaturas.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.CheckoutId == checkoutId);

        private Task<AssinaturaModel> BuscarPorAssinaturaExterna(string assinaturaExternaId)
            => string.IsNullOrWhiteSpace(assinaturaExternaId)
                ? Task.FromResult<AssinaturaModel>(null)
                : _context.Assinaturas.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.AssinaturaExternaId == assinaturaExternaId);

        private static int EmCentavos(decimal valor) => (int)(valor * 100);

        private static bool IguaisEmTempoConstante(string a, string b)
            => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));

        private static string Texto(JsonElement elemento, params string[] caminho)
        {
            foreach (var campo in caminho)
            {
                if (elemento.ValueKind != JsonValueKind.Object || !elemento.TryGetProperty(campo, out elemento))
                    return null;
            }

            return elemento.ValueKind == JsonValueKind.String ? elemento.GetString() : null;
        }
    }
}
