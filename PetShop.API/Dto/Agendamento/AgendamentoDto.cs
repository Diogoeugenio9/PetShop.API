namespace PetShop.API.Dto.Agendamento
{
    public class AgendamentoDto
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; }

        // Chaves estrangeiras
        public int PetId { get; set; }
        public int ServicoId { get; set; }

        // Informações adicionais (sem ciclo)
        public string? NomePet { get; set; }
        public string? NomeServico { get; set; }
    }
}
