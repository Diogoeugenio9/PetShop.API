using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Assinatura;
using PetShop.API.Services.Assinatura;
using PetShop.API.Utils;

namespace PetShop.API.Controllers
{
    [Route("api/admin/assinatura")]
    [ApiController]
    [Authorize(Policy = Politicas.Administrador)]
    public class AssinaturaController : ControllerBase
    {
        private readonly IAssinaturaService _assinaturaService;

        public AssinaturaController(IAssinaturaService assinaturaService)
        {
            _assinaturaService = assinaturaService;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutCriadoDto>> CriarCheckout(CheckoutAssinaturaDto dto)
        {
            return Ok(await _assinaturaService.CriarCheckout(User.ObterAdministradorId(), dto.Plano));
        }
    }
}
