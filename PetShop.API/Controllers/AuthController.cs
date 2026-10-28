using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PetShop.API.Dto;
using PetShop.API.Dto.LoginDto;
using PetShop.API.Dto.RegistroDto;
using PetShop.API.Services;
using PetShop.API.Utils;

namespace PetShop.API.Controllers
{
    [ApiController]
    [Route("api/admin/autenticacao")]
    [EnableRateLimiting("autenticacao")]
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
            var resultado = _authService.Login(dto);
            if (resultado == null)
                return Unauthorized(new RespostaErro("E-mail ou senha inválidos."));

            return Ok(Resposta(resultado));
        }

        [HttpPost("registrar")]
        public IActionResult Registrar(RegistroDto dto)
        {
            var resultado = _authService.Registrar(dto);
            if (resultado == null)
                return BadRequest(new RespostaErro("E-mail já cadastrado."));

            return Created("", Resposta(resultado));
        }

        private static object Resposta(AdminAutenticadoDto r) => new
        {
            token = r.Token,
            administrador = new { email = r.Email },
            petshopId = r.PetshopId
        };
    }
}
