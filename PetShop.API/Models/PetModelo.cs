using System.Text.Json.Serialization;

namespace PetShop.API.Models
{
    public class PetModelo
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public string Especie { get; set; }
        public string Raca { get; set; }
        public int Idade { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }

        [JsonIgnore]
        public bool Excluido { get; set; }

        public int ClienteId { get; set; }

        public virtual ClienteModel Cliente { get; set; }
    }
}
