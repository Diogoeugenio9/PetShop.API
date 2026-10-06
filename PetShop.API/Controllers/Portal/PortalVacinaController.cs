using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Vacina;
using PetShop.API.Services.Portal;
using PetShop.API.Utils;

namespace PetShop.API.Controllers.Portal
{
    [ApiController]
    [Route("api/Vacina")]
    [Authorize(Policy = Politicas.Cliente)]
    public class PortalVacinaController : ControllerBase
    {
        private readonly IPortalClienteService _portalService;

        public PortalVacinaController(IPortalClienteService portalService)
        {
            _portalService = portalService;
        }

        [HttpGet("MinhasVacinas")]
        public async Task<ActionResult<List<VacinaRespostaDto>>> MinhasVacinas()
        {
            return Ok(await _portalService.ListarMinhasVacinas(User.ObterClienteId()));
        }
    }
}
