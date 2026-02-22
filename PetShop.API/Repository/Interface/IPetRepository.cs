using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IPetRepository
    {
        Task<List<PetModel>> GetAll();
        Task<PetModel?> GetById(int id);
        Task<List<PetModel>> GetByClienteId(int clienteId);

        Task Add(PetModel pet);
        Task Update(PetModel pet);
        Task Delete(PetModel pet);
    }
}
