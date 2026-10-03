using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Pet
{
    public class PetModeloDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do pet.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(50)]
        public string Especie { get; set; }

        [StringLength(100)]
        public string Raca { get; set; }

        [Range(0, 50, ErrorMessage = "Idade deve estar entre 0 e 50.")]
        public int Idade { get; set; }

        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Informe o cliente do pet.")]
        public int ClienteId { get; set; }
    }
}
