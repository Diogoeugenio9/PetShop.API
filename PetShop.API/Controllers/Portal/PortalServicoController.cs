using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Models;
using PetShop.API.Services.Portal;

namespace PetShop.API.Controllers.Portal
{
    [ApiController]
    [Route("api/Servico")]
    [Authorize]
    public class PortalServicoController : ControllerBase
    {
        private readonly IPortalClienteService _portalService;

        public PortalServicoController(IPortalClienteService portalService)
        {
            _portalService = portalService;
        }

        [HttpGet("ListarServicosAtivos")]
        public async Task<ActionResult<List<ServicoModel>>> ListarServicosAtivos()
        {
            return Ok(await _portalService.ListarServicosAtivos());
        }
    }
}
