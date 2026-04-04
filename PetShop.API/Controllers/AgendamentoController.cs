using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Agendamento;
using PetShop.API.Models;
using PetShop.API.Services.Agendamento;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentoController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        
        [HttpGet("ListarAgendamentos")]
        public async Task<ActionResult<List<AgendamentoDto>>> ListarAgendamentos()
        {
            var agendamentos = await _agendamentoService.ListarAgendamentos();
            return Ok(agendamentos);
        }


       
        [HttpGet("BuscarAgendamentoPorId/{idAgendamento}")]
        public async Task<ActionResult<AgendamentoDto>> BuscarAgendamentoPorId(int idAgendamento)
        {
            var agendamento = await _agendamentoService.BuscarAgendamentoPorId(idAgendamento);
            if (agendamento == null)
                return NotFound("Agendamento não encontrado");

            return Ok(agendamento);
        }


        
        [HttpGet("BuscarPorPet/{petId}")]
        public async Task<ActionResult<List<AgendamentoDto>>> BuscarPorPet(int petId)
        {
            var agendamentos = await _agendamentoService.BuscarAgendamentosPorPet(petId);
            if (agendamentos == null || !agendamentos.Any())
                return NotFound("Nenhum agendamento encontrado para este pet");

            return Ok(agendamentos);
        }


        
        [HttpGet("BuscarPorCliente/{clienteId}")]
        public async Task<ActionResult<List<AgendamentoDto>>> BuscarPorCliente(int clienteId)
        {
            var agendamentos = await _agendamentoService.BuscarAgendamentosPorCliente(clienteId);
            if (agendamentos == null || !agendamentos.Any())
                return NotFound("Nenhum agendamento encontrado para este cliente");

            return Ok(agendamentos);
        }



        [HttpPost("CriarAgendamento")]
        public async Task<ActionResult<AgendamentoDto>> CriarAgendamento(AgendamentoCreateDto dto)
        {
            var agendamento = await _agendamentoService.CriarAgendamento(dto);
            return CreatedAtAction(nameof(BuscarAgendamentoPorId), new { idAgendamento = agendamento.Id }, agendamento);
        }




        [HttpPut("EditarAgendamento")]
        public async Task<ActionResult<AgendamentoDto>> EditarAgendamento(AgendamentoDto agendamentoEdicaoDto)
        {
            var agendamento = await _agendamentoService.EditarAgendamento(agendamentoEdicaoDto);
            if (agendamento == null)
                return NotFound("Agendamento não encontrado");

            return Ok(agendamento);
        }


        
        [HttpPatch("AlterarStatus/{idAgendamento}")]
        public async Task<IActionResult> AlterarStatus(int idAgendamento, [FromBody] string novoStatus)
        {
            var sucesso = await _agendamentoService.AlterarStatus(idAgendamento, novoStatus);
            if (!sucesso)
                return NotFound("Agendamento não encontrado");

            return NoContent();
        }

        
        [HttpDelete("ExcluirAgendamento/{idAgendamento}")]
        public async Task<IActionResult> ExcluirAgendamento(int idAgendamento)
        {
            var sucesso = await _agendamentoService.ExcluirAgendamento(idAgendamento);
            if (!sucesso)
                return NotFound("Agendamento não encontrado");

            return NoContent();
        }
    }
}
