using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto;
using PetShop.API.Dto.LoginDto;
using PetShop.API.Dto.RegistroDto;
using PetShop.API.Services;

namespace PetShop.API.Controllers
{
    [ApiController]
    [Route("api/admin/autenticacao")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AutenticacaoController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var token = _authService.Login(dto);
            if (token == null)
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

            return Ok(new { token, administrador = new { email = dto.Email } });
        }

        [HttpPost("registrar")]
        public IActionResult Registrar(RegistroDto dto)
        {
            var token = _authService.Registrar(dto);
            if (token == null)
                return BadRequest(new { mensagem = "E-mail já cadastrado." });

            return Created("", new { token, administrador = new { email = dto.Email } });
        }
    }
}
