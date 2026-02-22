using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Services.Pet
{
    public interface IPetService
    {
        // Consultas
        Task<ResponseModel<List<PetModel>>> ListarPets();
        Task<ResponseModel<PetModel>> BuscarPetPorId(int idPet);
        Task<ResponseModel<List<PetModel>>> BuscarPetPorIdCliente(int idCliente);

        // Comandos
        Task<ResponseModel<List<PetModel>>> CriarPet(PetCriacaoDto petCriacaoDto);
        Task<ResponseModel<List<PetModel>>> EditarPet(PetEdicaoDto petEdicaoDto);
        Task<ResponseModel<List<PetModel>>> ExcluirPet(int idPet);
    }
}
