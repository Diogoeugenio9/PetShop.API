using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Portal;
using PetShop.API.Services.Portal;
using PetShop.API.Utils;

namespace PetShop.API.Controllers.Portal
{
    [ApiController]
    [Route("api/Cliente")]
    [Authorize(Policy = Politicas.Cliente)]
    public class PortalPerfilController : ControllerBase
    {
        private readonly IPortalClienteService _portalService;

        public PortalPerfilController(IPortalClienteService portalService)
        {
            _portalService = portalService;
        }

        [HttpGet("MeuPerfil")]
        public async Task<ActionResult<PerfilClienteDto>> MeuPerfil()
        {
            return Ok(await _portalService.ObterPerfil(User.ObterClienteId()));
        }

        [HttpPut("EditarMeuPerfil")]
        public async Task<ActionResult<PerfilClienteDto>> EditarMeuPerfil(PerfilClienteDto dto)
        {
            return Ok(await _portalService.EditarPerfil(User.ObterClienteId(), dto));
        }
    }
}
