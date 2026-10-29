using PetShop.API.Dto.Assinatura;

namespace PetShop.API.Services.Assinatura
{
    public interface IAssinaturaService
    {
        Task<AssinaturaDto> Obter(int administradorId);
        Task<CheckoutCriadoDto> CriarCheckout(int administradorId, string plano);
        Task<AssinaturaDto> CancelarRenovacao(int administradorId);
        Task<AssinaturaDto> ReativarRenovacao(int administradorId);
        Task<int> EncerrarRenovacoesCanceladas(CancellationToken cancellationToken);
    }
}
