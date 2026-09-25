using System.Text.Json.Serialization;

namespace PetShop.API.Dto.Dashboard
{
    public class MetaMensalDto
    {
        [JsonPropertyName("3m")]
        public MetaDto TresMeses { get; set; }

        [JsonPropertyName("6m")]
        public MetaDto SeisMeses { get; set; }

        [JsonPropertyName("1a")]
        public MetaDto UmAno { get; set; }
    }

    public class MetaDto
    {
        public decimal Meta { get; set; }
        public decimal Realizado { get; set; }
    }
}