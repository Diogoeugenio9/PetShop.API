using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IPetModeloRepository
    {
        Task<List<PetModelo>> GetAllAsync();
        Task<PetModelo?> GetByIdAsync(int id);
        Task<List<PetModelo>> GetByClienteIdAsync(int clienteId);

        Task AddAsync(PetModelo pet);
        Task UpdateAsync(PetModelo pet);
        Task DeleteAsync(PetModelo pet);
    }

}