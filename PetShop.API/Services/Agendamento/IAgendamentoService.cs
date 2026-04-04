using PetShop.API.Dto.Agendamento;

namespace PetShop.API.Services.Agendamento
{
    public interface IAgendamentoService
    {
        
        Task<List<AgendamentoDto>> ListarAgendamentos();
        Task<AgendamentoDto?> BuscarAgendamentoPorId(int idAgendamento);
        Task<List<AgendamentoDto>> BuscarAgendamentosPorPet(int petId);
        Task<List<AgendamentoDto>> BuscarAgendamentosPorCliente(int clienteId);

        
        Task<AgendamentoDto> CriarAgendamento(AgendamentoCreateDto dto);

        Task<AgendamentoDto?> EditarAgendamento(AgendamentoDto agendamentoEdicaoDto);
        Task<bool> ExcluirAgendamento(int idAgendamento);

        
        Task<bool> AlterarStatus(int idAgendamento, string novoStatus);
    }
}
