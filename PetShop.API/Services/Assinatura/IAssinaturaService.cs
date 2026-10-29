using PetShop.API.Dto.Assinatura;

namespace PetShop.API.Services.Assinatura
{
    public interface IAssinaturaService
    {
        Task<CheckoutCriadoDto> CriarCheckout(int administradorId, string plano);
    }
}
