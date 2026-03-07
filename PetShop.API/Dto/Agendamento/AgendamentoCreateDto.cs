namespace PetShop.API.Dto.Agendamento
{
    public class AgendamentoCreateDto
    {
        public DateTime DataHora { get; set; }
        public int PetId { get; set; }
        public int ServicoId { get; set; }
    }
}
