using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Portal
{
    public class MeuPetDto
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

        [StringLength(20)]
        public string Sexo { get; set; }

        [Range(0, 999, ErrorMessage = "Peso inválido.")]
        public decimal? Peso { get; set; }

        [StringLength(500)]
        public string Observacoes { get; set; }
    }
}
