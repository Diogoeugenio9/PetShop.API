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
        public async Task<ActionResult<List<ClienteModel>>> ListarClientes()
        {
            var clientes = await _clienteInterface.ListarClientes();
            return Ok(clientes); 
        }

        [HttpGet("BuscarClientePorId/{idCliente}")] 
        public async Task<ActionResult<ClienteModel>> BuscarClientePorId(int idCliente) 
        {
            var cliente = await _clienteInterface.BuscarClientePorId(idCliente);
            if (cliente == null)
                return NotFound("Cliente não encontrado");
            return Ok(cliente); 
        }


        [HttpPost("CriarCliente")]
        public async Task<ActionResult<ClienteModel>> CriarCliente(ClienteCriacaoDto dto)
        {
            var cliente = await _clienteInterface.CriarCliente(dto);
            return CreatedAtAction(nameof(BuscarClientePorId), new { idCliente = cliente.Id }, cliente);
        }

        [HttpPut("EditarCliente")]
        public async Task<ActionResult<ClienteModel>> EditarCliente(ClienteEdicaoDto dto) 
        {
            var cliente = await _clienteInterface.EditarCliente(dto);
            if (cliente == null) 
                return NotFound("Cliente não encontrado");

            return Ok(cliente);
        }

        [HttpDelete("ExcluirCliente/{idCliente}")]
        public async Task<IActionResult> ExcluirCliente(int idCliente)
        {
            var sucesso = await _clienteInterface.ExcluirCliente(idCliente);
            if (!sucesso)
                return NotFound("Cliente não encontrado");

            return NoContent(); }
    }




}
