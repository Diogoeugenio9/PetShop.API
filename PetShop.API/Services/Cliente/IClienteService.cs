using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Services.Cliente
{
    public interface IClienteService
    {
        // Consultas
         Task<List<ClienteModel>> ListarClientes();
        Task<ClienteModel?> BuscarClientePorId(int idCliente);
        Task<ClienteModel?> BuscarClientePorIdPet(int idPet); 
        
        // Comandos
         Task<ClienteModel> CriarCliente(ClienteCriacaoDto clienteCriacaoDto);
        Task<ClienteModel?> EditarCliente(ClienteEdicaoDto clienteEdicaoDto);
        Task<bool> ExcluirCliente(int idCliente);
    }
}
