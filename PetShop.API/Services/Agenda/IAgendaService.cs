using PetShop.API.Models;

namespace PetShop.API.Services.Agenda
{
    public interface IAgendaService
    {
        Task<List<string>> HorariosDisponiveis(int administradorId, DateOnly data, int servicoId, int? agendamentoIgnoradoId);

        Task<T> ExecutarComTravaDaAgenda<T>(int administradorId, Func<Task<T>> acao);

        Task GarantirVaga(int administradorId, DateTime inicio, int duracaoMinutos, int? agendamentoIgnoradoId, ConfiguracaoLojaModel configuracao);

        void ValidarHorarioDoPortal(DateTime inicio, int duracaoMinutos, ConfiguracaoLojaModel configuracao);
    }
}
