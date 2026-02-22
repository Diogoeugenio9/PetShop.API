using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Services.Cliente
{
    public interface IClienteService
    {
        // Consultas
        Task<ResponseModel<List<ClienteModel>>> ListarClientes();
        Task<ResponseModel<ClienteModel>> BuscarClientePorId(int idCliente);
        Task<ResponseModel<ClienteModel>> BuscarClientePorIdPet(int idPet);

        // Comandos
        Task<ResponseModel<List<ClienteCriacaoDto>>> CriarCliente(ClienteCriacaoDto clienteCriacaoDto);
        Task<ResponseModel<List<ClienteEdicaoDto>>> EditarCliente(ClienteEdicaoDto clienteEdicaoDto);
        Task<ResponseModel<List<ClienteModel>>> ExcluirCliente(int idCliente);
    }
}
