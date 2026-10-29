using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Assinatura;
using PetShop.API.Models;
using PetShop.API.Services.Pagamento;
using PetShop.API.Utils;

namespace PetShop.API.Services.Assinatura
{
    public class AssinaturaService : IAssinaturaService
    {
        private static readonly TimeSpan AntecedenciaDoEncerramento = TimeSpan.FromDays(2);

        private readonly AppDbContext _context;
        private readonly IAbacatePayClient _abacatePay;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AssinaturaService> _logger;

        public AssinaturaService(AppDbContext context, IAbacatePayClient abacatePay, IConfiguration configuration, ILogger<AssinaturaService> logger)
        {
            _context = context;
            _abacatePay = abacatePay;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AssinaturaDto> Obter(int administradorId)
        {
            var vigente = await BuscarVigente(administradorId);
            if (vigente != null)
                return ParaDto(vigente, vigente.Status);

            var ultima = await _context.Assinaturas
                .Where(a => a.AdministradorId == administradorId)
                .OrderByDescending(a => a.CriadaEm)
                .FirstOrDefaultAsync();

            if (ultima == null)
                return new AssinaturaDto();

            var status = ultima.Status == StatusAssinatura.Ativa ? StatusAssinatura.Expirada : ultima.Status;
            return ParaDto(ultima, status);
        }

        public async Task<CheckoutCriadoDto> CriarCheckout(int administradorId, string plano)
        {
            plano = plano?.Trim().ToLowerInvariant();
            if (!PlanoAssinatura.EhValido(plano))
                throw new RegraDeNegocioException("Plano inválido. Use mensal ou vitalicio.");

            var vigente = await BuscarVigente(administradorId);

            if (vigente?.Plano == PlanoAssinatura.Vitalicio)
                throw new ConflitoException("Este petshop já possui a licença vitalícia.");

            if (vigente?.Plano == PlanoAssinatura.Mensal && plano == PlanoAssinatura.Mensal)
            {
                if (!vigente.RenovacaoCancelada)
                    throw new ConflitoException("Este petshop já possui uma assinatura mensal ativa.");

                throw new ConflitoException(vigente.CanceladaEm.HasValue
                    ? $"A assinatura mensal vale até {vigente.PagoAte:dd/MM/yyyy}. Contrate novamente depois dessa data."
                    : "A assinatura mensal ainda está vigente. Reative a renovação em vez de contratar de novo.");
            }

            var referencia = $"petshop-{administradorId}-{plano}-{Guid.NewGuid():N}";
            var (urlConclusao, urlRetorno) = UrlsDoFront();

            var checkout = plano == PlanoAssinatura.Mensal
                ? await _abacatePay.CriarCheckoutAssinatura(ProdutoDo(plano), referencia, urlConclusao, urlRetorno)
                : await _abacatePay.CriarCheckout(ProdutoDo(plano), referencia, urlConclusao, urlRetorno);

            if (string.IsNullOrWhiteSpace(checkout?.Id) || string.IsNullOrWhiteSpace(checkout.Url))
                throw new PagamentoIndisponivelException("O serviço de pagamento não retornou o link do checkout.");

            _context.Assinaturas.Add(new AssinaturaModel
            {
                AdministradorId = administradorId,
                Plano = plano,
                Status = StatusAssinatura.Pendente,
                Valor = PlanoAssinatura.ValorDe(plano),
                CheckoutId = checkout.Id,
                CriadaEm = DataHoraBrasil.Agora
            });
            await _context.SaveChangesAsync();

            return new CheckoutCriadoDto { Url = checkout.Url };
        }

        public async Task<AssinaturaDto> CancelarRenovacao(int administradorId)
        {
            var mensal = await BuscarMensalVigente(administradorId);

            if (!mensal.RenovacaoCancelada)
            {
                mensal.RenovacaoCancelada = true;
                await _context.SaveChangesAsync();
            }

            return ParaDto(mensal, mensal.Status);
        }

        public async Task<AssinaturaDto> ReativarRenovacao(int administradorId)
        {
            var mensal = await BuscarMensalVigente(administradorId);

            if (mensal.CanceladaEm.HasValue)
                throw new RegraDeNegocioException(
                    $"A renovação já foi encerrada no serviço de pagamento. A assinatura vale até {mensal.PagoAte:dd/MM/yyyy}.");

            if (mensal.RenovacaoCancelada)
            {
                mensal.RenovacaoCancelada = false;
                await _context.SaveChangesAsync();
            }

            return ParaDto(mensal, mensal.Status);
        }

        public async Task<int> EncerrarRenovacoesCanceladas(CancellationToken cancellationToken)
        {
            var limite = DataHoraBrasil.Agora.Add(AntecedenciaDoEncerramento);

            var pendentes = await _context.Assinaturas
                .IgnoreQueryFilters()
                .Where(a => a.Plano == PlanoAssinatura.Mensal
                            && a.Status == StatusAssinatura.Ativa
                            && a.RenovacaoCancelada
                            && a.CanceladaEm == null
                            && a.AssinaturaExternaId != null
                            && a.PagoAte <= limite)
                .ToListAsync(cancellationToken);

            var encerradas = 0;
            foreach (var assinatura in pendentes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await _abacatePay.CancelarAssinatura(assinatura.AssinaturaExternaId);
                    assinatura.CanceladaEm = DataHoraBrasil.Agora;
                    await _context.SaveChangesAsync(cancellationToken);
                    encerradas++;
                }
                catch (PagamentoIndisponivelException ex)
                {
                    _logger.LogError(ex, "Falha ao encerrar a renovação da assinatura {AssinaturaId}.", assinatura.AssinaturaExternaId);
                }
            }

            return encerradas;
        }

        private async Task<AssinaturaModel> BuscarMensalVigente(int administradorId)
        {
            var vigente = await BuscarVigente(administradorId);

            if (vigente == null)
                throw new RegraDeNegocioException("Este petshop não possui assinatura ativa.");

            if (vigente.Plano == PlanoAssinatura.Vitalicio)
                throw new RegraDeNegocioException("A licença vitalícia não tem renovação.");

            return vigente;
        }

        private static AssinaturaDto ParaDto(AssinaturaModel assinatura, string status) => new AssinaturaDto
        {
            Plano = assinatura.Plano,
            Status = status,
            RenovacaoAutomatica = assinatura.Plano == PlanoAssinatura.Mensal
                                  && status == StatusAssinatura.Ativa
                                  && !assinatura.RenovacaoCancelada,
            ContratadaEm = assinatura.AtivadaEm,
            PagoAte = assinatura.Plano == PlanoAssinatura.Vitalicio ? null : assinatura.PagoAte
        };

        private async Task<AssinaturaModel> BuscarVigente(int administradorId)
        {
            var agora = DataHoraBrasil.Agora;

            return await _context.Assinaturas
                .Where(a => a.AdministradorId == administradorId
                            && a.Status == StatusAssinatura.Ativa
                            && (a.Plano == PlanoAssinatura.Vitalicio || a.PagoAte >= agora))
                .OrderByDescending(a => a.Plano == PlanoAssinatura.Vitalicio)
                .ThenByDescending(a => a.PagoAte)
                .FirstOrDefaultAsync();
        }

        private string ProdutoDo(string plano)
        {
            var chave = plano == PlanoAssinatura.Mensal ? "AbacatePay:ProdutoMensalId" : "AbacatePay:ProdutoVitalicioId";
            var produtoId = _configuration[chave];

            if (string.IsNullOrWhiteSpace(produtoId))
                throw new PagamentoIndisponivelException("O produto de cobrança deste plano não foi configurado.");

            return produtoId;
        }

        private (string conclusao, string retorno) UrlsDoFront()
        {
            var front = _configuration["AbacatePay:UrlFront"]?.TrimEnd('/');
            if (string.IsNullOrWhiteSpace(front))
                return (null, null);

            return ($"{front}/admin/assinatura", $"{front}/planos");
        }
    }
}
