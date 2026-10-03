using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetShop.API.Models
{
    public class ServicoModel : IPertenceALoja
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        public int DuracaoMinutos { get; set; }
        public bool Ativo { get; set; }

        [JsonIgnore]
        public bool Excluido { get; set; }

        [JsonIgnore]
        public int AdministradorId { get; set; }

        [JsonIgnore]
        public virtual ICollection<AgendamentoModel> Agendamentos { get; set; }
    }
}
