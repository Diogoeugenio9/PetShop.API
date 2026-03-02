using PetShop.API.Dto.Cliente;

namespace PetShop.API.Services.Cliente
{
    public interface IClienteService
    {
        // Consultas
        Task<List<ClienteDto>> ListarClientes();
        Task<ClienteDto?> BuscarClientePorId(int idCliente);
        Task<ClienteDto?> BuscarClientePorIdPet(int idPet);

        // Comandos
        Task<ClienteDto> CriarCliente(ClienteDto clienteDto);
        Task<ClienteDto?> EditarCliente(ClienteDto clienteDto);
        Task<bool> ExcluirCliente(int idCliente);
    }
}
