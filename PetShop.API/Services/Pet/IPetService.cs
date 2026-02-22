using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Services.Pet
{
    public interface IPetService
    {
        // Consultas
        Task<List<PetModel>> ListarPets();
        Task<PetModel?> BuscarPetPorId(int idPet);
        Task<List<PetModel>> BuscarPetPorIdCliente(int idCliente);

        // Comandos
        Task<PetModel> CriarPet(PetCriacaoDto petCriacaoDto); 
        Task<PetModel?> EditarPet(PetEdicaoDto petEdicaoDto);
        Task<bool> ExcluirPet(int idPet);
    }
}
