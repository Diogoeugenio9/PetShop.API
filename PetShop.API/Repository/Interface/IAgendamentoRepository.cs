using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IAgendamentoRepository
    {
        Task<List<AgendamentoModel>> GetAllAsync();
        Task<AgendamentoModel?> GetByIdAsync(int id);
        Task<List<AgendamentoModel>> GetByPetIdAsync(int petId);
        Task<List<AgendamentoModel>> GetByClienteIdAsync(int clienteId);
        Task<AgendamentoModel> AddAsync(AgendamentoModel agendamento);
        Task<AgendamentoModel> UpdateAsync(AgendamentoModel agendamento);
        Task<bool> DeleteAsync(int id);

        // metodos auxiliares
        Task<PetModelo?> GetPetByIdAsync(int petId);
        Task<ServicoModel?> GetServicoByIdAsync(int servicoId);
    }
}
