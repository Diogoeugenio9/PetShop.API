using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Services.Pet
{
    public interface IPetModeloService
    {
        //OOMADOS PARA CONSULTA DE PETS
        Task<List<PetModel>> ListarPets();
        Task<PetModel?> BuscarPetPorId(int idPet);
        Task<List<PetModel>> BuscarPetPorIdCliente(int idCliente);

        //COMANDOS PARA PODER REALIZAR AS OPERAÇÕES DE CRIAÇÃO, EDIÇÃO E EXCLUSÃO DE PETS
        Task<PetModel> CriarPet(PetModeloDto petCriacaoDto);
        Task<PetModel?> EditarPet(PetModeloDto petEdicaoDto);
        Task<bool> ExcluirPet(int idPet);
    }
}