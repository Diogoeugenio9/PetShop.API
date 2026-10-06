using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Vacina;
using PetShop.API.Services.Vacina;
using PetShop.API.Utils;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Politicas.Administrador)]
    public class VacinaController : ControllerBase
    {
        private readonly IVacinaService _vacinaService;

        public VacinaController(IVacinaService vacinaService)
        {
            _vacinaService = vacinaService;
        }

        [HttpGet("ListarVacinas")]
        public async Task<ActionResult<List<VacinaRespostaDto>>> ListarVacinas()
        {
            return Ok(await _vacinaService.ListarVacinas());
        }

        [HttpGet("BuscarVacinaPorId/{id}")]
        public async Task<ActionResult<VacinaRespostaDto>> BuscarVacinaPorId(int id)
        {
            var vacina = await _vacinaService.BuscarVacinaPorId(id);
            if (vacina == null)
                return NotFound(new RespostaErro("Vacina não encontrada."));

            return Ok(vacina);
        }

        [HttpPost("CriarVacina")]
        public async Task<ActionResult<VacinaRespostaDto>> CriarVacina(VacinaDto dto)
        {
            var vacina = await _vacinaService.CriarVacina(dto);
            return CreatedAtAction(nameof(BuscarVacinaPorId), new { id = vacina.Id }, vacina);
        }

        [HttpPut("EditarVacina")]
        public async Task<ActionResult<VacinaRespostaDto>> EditarVacina(VacinaDto dto)
        {
            var vacina = await _vacinaService.EditarVacina(dto);
            if (vacina == null)
                return NotFound(new RespostaErro("Vacina não encontrada."));

            return Ok(vacina);
        }

        [HttpDelete("ExcluirVacina/{id}")]
        public async Task<IActionResult> ExcluirVacina(int id)
        {
            var sucesso = await _vacinaService.ExcluirVacina(id);
            if (!sucesso)
                return NotFound(new RespostaErro("Vacina não encontrada."));

            return NoContent();
        }
    }
}
