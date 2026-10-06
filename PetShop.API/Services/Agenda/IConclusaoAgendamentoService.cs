namespace PetShop.API.Services.Agenda
{
    public enum ResultadoConclusao
    {
        Concluido,
        JaEstavaConcluido,
        NaoEncontrado,
        Cancelado,
        HorarioAindaNaoChegou,
        StatusNaoPermite
    }

    public interface IConclusaoAgendamentoService
    {
        Task<ResultadoConclusao> Concluir(int agendamentoId);

        Task<int> ConcluirVencidos(CancellationToken cancellationToken);

        string MensagemDe(ResultadoConclusao resultado);
    }
}
