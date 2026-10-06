using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Vacina
{
    public class VacinaDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Informe o pet.")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Informe o nome da vacina.")]
        [StringLength(100)]
        public string NomeVacina { get; set; }

        [StringLength(100)]
        public string Fabricante { get; set; }

        [StringLength(50)]
        public string Lote { get; set; }

        public DateOnly DataAplicacao { get; set; }

        public DateOnly? DataProximaDose { get; set; }

        public bool DoseUnica { get; set; }

        [StringLength(500)]
        public string Observacoes { get; set; }
    }

    public class VacinaRespostaDto
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public string PetNome { get; set; }
        public string ClienteNome { get; set; }
        public string NomeVacina { get; set; }
        public string Fabricante { get; set; }
        public string Lote { get; set; }
        public DateOnly DataAplicacao { get; set; }
        public DateOnly? DataProximaDose { get; set; }
        public bool DoseUnica { get; set; }
        public string Observacoes { get; set; }
    }
}
