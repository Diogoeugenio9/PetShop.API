using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PetShop.API.Dto.ClienteAutenticacao;
using PetShop.API.Services.ClienteAutenticacao;
using PetShop.API.Utils;

namespace PetShop.API.Controllers
{
    [ApiController]
    [Route("api/cliente/autenticacao")]
    [AllowAnonymous]
    [EnableRateLimiting("autenticacao")]
    public class ClienteAutenticacaoController : ControllerBase
    {
        private readonly IClienteAuthService _clienteAuthService;

        public ClienteAutenticacaoController(IClienteAuthService clienteAuthService)
        {
            _clienteAuthService = clienteAuthService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ClienteAutenticadoDto>> Login(ClienteLoginDto dto)
        {
            var resultado = await _clienteAuthService.Login(dto);
            if (resultado == null)
                return Unauthorized(new RespostaErro("E-mail ou senha inválidos."));

            return Ok(resultado);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<ClienteAutenticadoDto>> Registrar(ClienteRegistroDto dto)
        {
            var resultado = await _clienteAuthService.Registrar(dto);
            return Created(string.Empty, resultado);
        }

        [HttpGet("petshops")]
        public async Task<ActionResult<List<PetshopPublicoDto>>> ListarPetshops()
        {
            return Ok(await _clienteAuthService.ListarPetshops());
        }
    }
}
