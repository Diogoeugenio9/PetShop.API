using PetShop.API.Dto.Vinculo;
using PetShop.API.Models;

namespace PetShop.API.Dto.Pet
{
    public class PetEdicaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public string Especie { get; set; }
        public string Raca { get; set; }
        public int Idade { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }


        public int ClienteId { get; set; }

    }
}
