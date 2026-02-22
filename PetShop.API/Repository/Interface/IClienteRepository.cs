using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IClienteRepository
    {
        public interface IClienteRepository
        {
            Task<List<ClienteModel>> GetAll();
            Task<ClienteModel?> GetById(int id);
            Task<ClienteModel?> GetByPetId(int petId);
            Task Add(ClienteModel cliente);
            Task Update(ClienteModel cliente);
            Task Delete(ClienteModel cliente);
        }
    }
}
