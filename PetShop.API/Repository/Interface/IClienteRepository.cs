using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IClienteRepository
    {
        Task<List<ClienteModel>> GetAllAsync();
        Task<ClienteModel?> GetByIdAsync(int id);
        Task<ClienteModel?> GetByPetId(int petId);
        Task AddAsync(ClienteModel cliente);
        Task UpdateAsync(ClienteModel cliente);
        Task Delete(ClienteModel cliente);
    }
}
