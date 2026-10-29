using System.Text.Json.Serialization;

namespace PetShop.API.Dto.Assinatura
{
    public class AssinaturaDto
    {
        public string Plano { get; set; }
        public string Status { get; set; }
        public bool RenovacaoAutomatica { get; set; }
        public DateTime? ContratadaEm { get; set; }

        [JsonPropertyName("periodEndsAt")]
        public DateTime? PagoAte { get; set; }
    }
}
