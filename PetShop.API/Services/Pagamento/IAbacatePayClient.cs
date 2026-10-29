namespace PetShop.API.Services.Pagamento
{
    public interface IAbacatePayClient
    {
        Task<CheckoutGateway> CriarCheckout(string produtoId, string referencia, string urlConclusao, string urlRetorno);
        Task<CheckoutGateway> CriarCheckoutAssinatura(string produtoId, string referencia, string urlConclusao, string urlRetorno);
        Task<CheckoutGateway> BuscarCheckout(string checkoutId);
        Task<AssinaturaGateway> BuscarAssinaturaDoCheckout(string checkoutId);
        Task CancelarAssinatura(string assinaturaId);
    }
}
