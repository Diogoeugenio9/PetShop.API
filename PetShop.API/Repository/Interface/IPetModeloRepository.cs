using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IPetModeloRepository
    {
        Task<List<PetModel>> GetAllAsync();
        Task<PetModel?> GetByIdAsync(int id);
        Task<List<PetModel>> GetByClienteIdAsync(int clienteId);

        Task AddAsync(PetModel pet);
        Task UpdateAsync(PetModel pet);
        Task DeleteAsync(PetModel pet);
    }

}