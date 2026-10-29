namespace PetShop.API.Services.Pagamento
{
    public interface IWebhookPagamentoService
    {
        bool OrigemValida(string corpo, string assinatura, string segredo);
        Task Processar(string corpo);
    }
}
