using PetShop.API.Dto.Agendamento;

namespace PetShop.API.Services.Agendamento
{
    public interface IAgendamentoService
    {
        // Consultas
        Task<List<AgendamentoDto>> ListarAgendamentos();
        Task<AgendamentoDto?> BuscarAgendamentoPorId(int idAgendamento);
        Task<List<AgendamentoDto>> BuscarAgendamentosPorPet(int petId);
        Task<List<AgendamentoDto>> BuscarAgendamentosPorCliente(int clienteId);

        // Operações CRUD
        Task<AgendamentoDto> CriarAgendamento(AgendamentoCreateDto dto);

        Task<AgendamentoDto?> EditarAgendamento(AgendamentoDto agendamentoEdicaoDto);
        Task<bool> ExcluirAgendamento(int idAgendamento);

        // Operação específica
        Task<bool> AlterarStatus(int idAgendamento, string novoStatus);
    }
}
