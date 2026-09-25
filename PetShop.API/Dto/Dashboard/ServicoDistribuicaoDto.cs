using System.Text.Json.Serialization;

namespace PetShop.API.Dto.Dashboard
{
    public class ServicosDistribuicaoDto
    {
        [JsonPropertyName("3m")]
        public List<ServicoDistribuicaoItemDto> TresMeses { get; set; }

        [JsonPropertyName("6m")]
        public List<ServicoDistribuicaoItemDto> SeisMeses { get; set; }

        [JsonPropertyName("1a")]
        public List<ServicoDistribuicaoItemDto> UmAno { get; set; }
    }

    public class ServicoDistribuicaoItemDto
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
    }
}