namespace PetShop.API.Models
{
    public class PetModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }


        public int ClienteId { get; set; }
        public ClienteModel Cliente { get; set; }
    }
}
