using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IServicoRepository
    {

        Task<List<ServicoModel>> GetAllAsync();
        Task<ServicoModel?> GetByIdAsync(int id);

        Task<ServicoModel> AddAsync(ServicoModel servico);
        Task<ServicoModel> UpdateAsync(ServicoModel servico);
        Task<bool> DeleteAsync(int id);
    }
}





//Task<List<ServicoModel>> GetByClienteIdAsync(int clienteId);
