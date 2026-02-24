using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Servico;
using PetShop.API.Models;
using PetShop.API.Services.Servico;
using PetShop.API.Services.Servico.PetShop.API.Services.Servico;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicoController : ControllerBase
    {
        private readonly IServicoService _servicoService;

        public ServicoController(IServicoService servicoService)
        {
            _servicoService = servicoService;
        }

        // Listar todos os serviços
        [HttpGet("ListarServicos")]
        public async Task<ActionResult<List<ServicoModel>>> ListarServicos()
        {
            var servicos = await _servicoService.ListarServicos();
            return Ok(servicos);
        }

        // Buscar serviço por ID
        [HttpGet("BuscarServicoPorId/{idServico}")]
        public async Task<ActionResult<ServicoModel>> BuscarServicoPorId(int idServico)
        {
            var servico = await _servicoService.BuscarServicoPorId(idServico);
            if (servico == null)
                return NotFound("Serviço não encontrado");

            return Ok(servico);
        }

        // Criar serviço
        [HttpPost("CriarServico")]
        public async Task<ActionResult<ServicoModel>> CriarServico(ServicoDto servicoCriacaoDto)
        {
            var servico = await _servicoService.CriarServico(servicoCriacaoDto);
            return CreatedAtAction(nameof(BuscarServicoPorId), new { idServico = servico.Id }, servico);
        }

        // Editar serviço
        [HttpPut("EditarServico")]
        public async Task<ActionResult<ServicoModel>> EditarServico(ServicoDto servicoEdicaoDto)
        {
            var servico = await _servicoService.EditarServico(servicoEdicaoDto);
            if (servico == null)
                return NotFound("Serviço não encontrado");

            return Ok(servico);
        }

        // Excluir serviço
        [HttpDelete("ExcluirServico/{idServico}")]
        public async Task<IActionResult> ExcluirServico(int idServico)
        {
            var sucesso = await _servicoService.ExcluirServico(idServico);
            if (!sucesso)
                return NotFound("Serviço não encontrado");

            return NoContent();
        }
    }
}
