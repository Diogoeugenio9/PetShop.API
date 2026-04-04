using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Services.Pet
{
    public interface IPetModeloService
    {
        
        Task<List<PetModelo>> ListarPets();
        Task<PetModelo?> BuscarPetPorId(int idPet);
        Task<List<PetModelo>> BuscarPetPorIdCliente(int idCliente);

        
        Task<PetModelo> CriarPet(PetModeloDto petCriacaoDto);
        Task<PetModelo?> EditarPet(PetModeloDto petEdicaoDto);
        Task<bool> ExcluirPet(int idPet);
    }
}