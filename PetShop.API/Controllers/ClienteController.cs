using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Cliente;
using PetShop.API.Services.Cliente;

namespace PetShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("ListarClientes")]
        public async Task<ActionResult<List<ClienteDto>>> ListarClientes()
        {
            var clientes = await _clienteService.ListarClientes();
            return Ok(clientes);
        }

        [HttpGet("BuscarClientePorId/{id}")]
        public async Task<ActionResult<ClienteDto>> BuscarClientePorId(int id)
        {
            var cliente = await _clienteService.BuscarClientePorId(id);
            if (cliente == null) return NotFound("Cliente não encontrado");
            return Ok(cliente);
        }

        [HttpPost("CriarCliente")]
        public async Task<ActionResult<ClienteDto>> CriarCliente(ClienteDto dto)
        {
            var cliente = await _clienteService.CriarCliente(dto);
            return CreatedAtAction(nameof(BuscarClientePorId), new { id = cliente.Id }, cliente);
        }

        [HttpPut("EditarCliente")]
        public async Task<ActionResult<ClienteDto>> EditarCliente(ClienteDto dto)
        {
            var cliente = await _clienteService.EditarCliente(dto);
            if (cliente == null) return NotFound("Cliente não encontrado");
            return Ok(cliente);
        }

        [HttpDelete("ExcluirCliente/{id}")]
        public async Task<ActionResult> ExcluirCliente(int id)
        {
            var sucesso = await _clienteService.ExcluirCliente(id);
            if (!sucesso) return NotFound("Cliente não encontrado");
            return NoContent();
        }
    }
}
