using System.Text.Json.Serialization;

namespace PetShop.API.Models
{
    public class ClienteModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }

        [JsonIgnore]
        public ICollection<PetModel> Pets { get; set; }
    }
}
