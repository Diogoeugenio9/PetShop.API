using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Portal
{
    public class MeuAgendamentoDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Informe o pet.")]
        public int PetId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Informe o serviço.")]
        public int ServicoId { get; set; }

        public DateTime DataHora { get; set; }

        [StringLength(500)]
        public string Observacoes { get; set; }
    }
}
