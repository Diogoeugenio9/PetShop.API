using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Portal;
using PetShop.API.Services.Portal;
using PetShop.API.Utils;

namespace PetShop.API.Controllers.Portal
{
    [ApiController]
    [Route("api/Pet")]
    [Authorize(Policy = Politicas.Cliente)]
    public class PortalPetController : ControllerBase
    {
        private readonly IPortalClienteService _portalService;

        public PortalPetController(IPortalClienteService portalService)
        {
            _portalService = portalService;
        }

        [HttpGet("MeusPets")]
        public async Task<ActionResult<List<MeuPetDto>>> MeusPets()
        {
            return Ok(await _portalService.ListarMeusPets(User.ObterClienteId()));
        }

        [HttpPost("CriarMeuPet")]
        public async Task<ActionResult<MeuPetDto>> CriarMeuPet(MeuPetDto dto)
        {
            var pet = await _portalService.CriarMeuPet(User.ObterClienteId(), dto);
            return Created(string.Empty, pet);
        }

        [HttpPut("EditarMeuPet")]
        public async Task<ActionResult<MeuPetDto>> EditarMeuPet(MeuPetDto dto)
        {
            var pet = await _portalService.EditarMeuPet(User.ObterClienteId(), dto);
            if (pet == null)
                return NotFound(new RespostaErro("Pet não encontrado."));

            return Ok(pet);
        }

        [HttpDelete("ExcluirMeuPet/{id}")]
        public async Task<IActionResult> ExcluirMeuPet(int id)
        {
            var sucesso = await _portalService.ExcluirMeuPet(User.ObterClienteId(), id);
            if (!sucesso)
                return NotFound(new RespostaErro("Pet não encontrado."));

            return NoContent();
        }
    }
}
