using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;
using PetShop.API.Services.Cliente;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {

        private readonly IClienteService _clienteInterface;
        public ClienteController(IClienteService clienteInterface)
        {
            _clienteInterface = clienteInterface;
        }

        [HttpGet("ListarClientes")]
        public async Task<ActionResult<ResponseModel<List<ClienteModel>>>> ListarClientes()
        {
            var clientes = await _clienteInterface.ListarClientes();
            return Ok(clientes);
        }

        [HttpGet("BuscarClientePorId/{idCliente}")]
        public async Task<ActionResult<ResponseModel<ClienteModel>>> BuscarClientePorId(int idCliente)
        {
            var cliente = await _clienteInterface.BuscarClientePorId(idCliente);
            return Ok(cliente);
        }

        [HttpGet("BuscarClientePorIdPet/{idPet}")]
        public async Task<ActionResult<ResponseModel<ClienteModel>>> BuscarClientePorIdPet(int idPet)
        {
            var cliente = await _clienteInterface.BuscarClientePorIdPet(idPet);
            return Ok(cliente);
        }

        [HttpPost("CriarCliente")]
        public async Task<ActionResult<ResponseModel<List<ClienteModel>>>> CriarCliente(ClienteCriacaoDto clienteCriacaoDto)
        {
            var clientes = await _clienteInterface.CriarCliente(clienteCriacaoDto);
            return Ok(clientes);
        }

        [HttpPut("EditarCliente")]
        public async Task<ActionResult<ResponseModel<List<ClienteModel>>>> EditarCliente(ClienteEdicaoDto clienteEdicaoDto)
        {
            var clientes = await _clienteInterface.EditarCliente(clienteEdicaoDto);
            return Ok(clientes);
        }

        [HttpDelete("ExcluirCliente")]
        public async Task<ActionResult<ResponseModel<List<ClienteModel>>>> ExcluirCliente(int idCliente)
        {
            var clientes = await _clienteInterface.ExcluirCliente(idCliente);
            return Ok(clientes);
        }
    }




}
