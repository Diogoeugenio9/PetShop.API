namespace PetShop.API.Models
{
    public class AssinaturaModel : IPertenceALoja
    {
        public int Id { get; set; }
        public int AdministradorId { get; set; }

        public string Plano { get; set; }
        public string Status { get; set; }
        public decimal Valor { get; set; }

        public string CheckoutId { get; set; }
        public string AssinaturaExternaId { get; set; }

        public bool RenovacaoCancelada { get; set; }

        public DateTime CriadaEm { get; set; }
        public DateTime? AtivadaEm { get; set; }
        public DateTime? PagoAte { get; set; }
        public DateTime? CanceladaEm { get; set; }
    }
}
