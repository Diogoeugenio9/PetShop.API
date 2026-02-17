using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Services.Cliente
{
    public interface IClienteInterface
    {
        Task<ResponseModel<List<ClienteModel>>> ListarClientes();
        Task<ResponseModel<ClienteModel>> BuscarClientePorId(int idCliente);
        Task<ResponseModel<ClienteModel>> BuscarClientePorIdPet(int idPet);


        Task<ResponseModel<List<ClienteModel>>> CriarCliente(ClienteCriacaoDto clienteCriacaoDto);
        Task<ResponseModel<List<ClienteModel>>> EditarCliente(ClienteEdicaoDto clienteEdicaoDto);
        Task<ResponseModel<List<ClienteModel>>> ExcluirCliente(int idCliente);
    }
}
