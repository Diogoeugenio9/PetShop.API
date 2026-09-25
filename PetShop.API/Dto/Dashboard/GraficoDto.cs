using System.Text.Json.Serialization;

namespace PetShop.API.Dto.Dashboard
{
    public class GraficoDto
    {
        [JsonPropertyName("3m")]
        public List<GraficoMesDto> TresMeses { get; set; }

        [JsonPropertyName("6m")]
        public List<GraficoMesDto> SeisMeses { get; set; }

        [JsonPropertyName("1a")]
        public List<GraficoMesDto> UmAno { get; set; }
    }

    public class GraficoMesDto
    {
        public string Mes { get; set; }
        public decimal Receitas { get; set; }
        public decimal Despesas { get; set; }
    }
}