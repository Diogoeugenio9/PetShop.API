namespace PetShop.API.Services.Pagamento
{
    public class CheckoutGateway
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public int ValorCentavos { get; set; }
    }
}
