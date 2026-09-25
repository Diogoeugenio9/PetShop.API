using System.Text.Json.Serialization;

namespace PetShop.API.Dto.Dashboard
{
    public class FaturamentoPorServicoDto
    {
        [JsonPropertyName("3m")]
        public List<FaturamentoServicoItemDto> TresMeses { get; set; }

        [JsonPropertyName("6m")]
        public List<FaturamentoServicoItemDto> SeisMeses { get; set; }

        [JsonPropertyName("1a")]
        public List<FaturamentoServicoItemDto> UmAno { get; set; }
    }

    public class FaturamentoServicoItemDto
    {
        public string Servico { get; set; }
        public decimal Faturamento { get; set; }
    }
}