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
        private readonly AppDbContext _context;
        private readonly IAbacatePayClient _abacatePay;
        private readonly IConfiguration _configuration;

        public AssinaturaService(AppDbContext context, IAbacatePayClient abacatePay, IConfiguration configuration)
        {
            _context = context;
            _abacatePay = abacatePay;
            _configuration = configuration;
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
                throw new ConflitoException(vigente.RenovacaoCancelada
                    ? "A assinatura mensal ainda está vigente. Reative a renovação em vez de contratar de novo."
                    : "Este petshop já possui uma assinatura mensal ativa.");

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
