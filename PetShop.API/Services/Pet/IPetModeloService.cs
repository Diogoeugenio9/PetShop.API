using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Services.Pet
{
    public interface IPetModeloService
    {
        //OOMADOS PARA CONSULTA DE PETS
        Task<List<PetModelo>> ListarPets();
        Task<PetModelo?> BuscarPetPorId(int idPet);
        Task<List<PetModelo>> BuscarPetPorIdCliente(int idCliente);

        //COMANDOS PARA PODER REALIZAR AS OPERAÇÕES DE CRIAÇÃO, EDIÇÃO E EXCLUSÃO DE PETS
        Task<PetModelo> CriarPet(PetModeloDto petCriacaoDto);
        Task<PetModelo?> EditarPet(PetModeloDto petEdicaoDto);
        Task<bool> ExcluirPet(int idPet);
    }
}