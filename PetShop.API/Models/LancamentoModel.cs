using System.Text.Json.Serialization;

namespace PetShop.API.Models
{
    public class LancamentoModel : IPertenceALoja
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }
        public DateTime Data { get; set; }
        public string Categoria { get; set; }
        public string FormaPagamento { get; set; }
        public string Observacoes { get; set; }

        [JsonIgnore]
        public int AdministradorId { get; set; }
    }
}
