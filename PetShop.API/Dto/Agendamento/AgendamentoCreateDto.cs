using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Agendamento
{
    public class AgendamentoCreateDto
    {
        public DateTime DataHora { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Informe o pet.")]
        public int PetId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Informe o serviço.")]
        public int ServicoId { get; set; }
    }
}
