using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Lancamento;
using PetShop.API.Models;
using PetShop.API.Services.Lancamento;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LancamentoController : ControllerBase
    {
        private readonly ILancamentoService _lancamentoService;

        public LancamentoController(ILancamentoService lancamentoService)
        {
            _lancamentoService = lancamentoService;
        }

        [HttpGet("ListarLancamentos")]
        public async Task<ActionResult<List<LancamentoModel>>> ListarLancamentos()
        {
            var lancamentos = await _lancamentoService.ListarLancamentos();
            return Ok(lancamentos);
        }

        [HttpGet("BuscarPorId/{id}")]
        public async Task<ActionResult<LancamentoModel>> BuscarPorId(int id)
        {
            var lancamento = await _lancamentoService.BuscarLancamentoPorId(id);

            if (lancamento == null)
                return NotFound("Lançamento não encontrado");

            return Ok(lancamento);
        }

        [HttpPost("CriarLancamento")]
        public async Task<ActionResult<LancamentoModel>> CriarLancamento(LancamentoDto lancamentoCriacaoDto)
        {
            var lancamento = await _lancamentoService.CriarLancamento(lancamentoCriacaoDto);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = lancamento.Id },
                lancamento
            );
        }

        [HttpPut("EditarLancamento")]
        public async Task<ActionResult<LancamentoModel>> EditarLancamento(LancamentoDto lancamentoEdicaoDto)
        {
            var lancamento = await _lancamentoService.EditarLancamento(lancamentoEdicaoDto);

            if (lancamento == null)
                return NotFound("Lançamento não encontrado");

            return Ok(lancamento);
        }

        [HttpDelete("ExcluirLancamento/{id}")]
        public async Task<IActionResult> ExcluirLancamento(int id)
        {
            var sucesso = await _lancamentoService.ExcluirLancamento(id);

            if (!sucesso)
                return NotFound("Lançamento não encontrado");

            return NoContent();
        }
    }
}