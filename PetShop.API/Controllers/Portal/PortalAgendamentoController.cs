using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Agendamento;
using PetShop.API.Dto.Portal;
using PetShop.API.Services.Agenda;
using PetShop.API.Services.Portal;
using PetShop.API.Utils;

namespace PetShop.API.Controllers.Portal
{
    [ApiController]
    [Route("api/Agendamento")]
    [Authorize]
    public class PortalAgendamentoController : ControllerBase
    {
        private readonly IPortalClienteService _portalService;
        private readonly IAgendaService _agendaService;

        public PortalAgendamentoController(IPortalClienteService portalService, IAgendaService agendaService)
        {
            _portalService = portalService;
            _agendaService = agendaService;
        }

        [HttpGet("MeusAgendamentos")]
        [Authorize(Policy = Politicas.Cliente)]
        public async Task<ActionResult<List<AgendamentoDto>>> MeusAgendamentos()
        {
            return Ok(await _portalService.ListarMeusAgendamentos(User.ObterClienteId()));
        }

        [HttpGet("HorariosDisponiveis")]
        public async Task<ActionResult<List<string>>> HorariosDisponiveis(
            [FromQuery] string data, [FromQuery] int servicoId, [FromQuery] int? agendamentoId)
        {
            if (!DateOnly.TryParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dia))
                return BadRequest(new RespostaErro("Informe a data no formato yyyy-MM-dd."));

            if (servicoId <= 0)
                return BadRequest(new RespostaErro("Informe o serviço."));

            var administradorId = User.ObterAdministradorId();
            if (administradorId == 0)
                return Forbid();

            var horarios = await _agendaService.HorariosDisponiveis(administradorId, dia, servicoId, agendamentoId);
            return Ok(horarios);
        }

        [HttpPost("CriarMeuAgendamento")]
        [Authorize(Policy = Politicas.Cliente)]
        public async Task<ActionResult<AgendamentoDto>> CriarMeuAgendamento(MeuAgendamentoDto dto)
        {
            var agendamento = await _portalService.CriarMeuAgendamento(User.ObterClienteId(), User.ObterPetshopId(), dto);
            return Created(string.Empty, agendamento);
        }

        [HttpPut("EditarMeuAgendamento")]
        [Authorize(Policy = Politicas.Cliente)]
        public async Task<ActionResult<AgendamentoDto>> EditarMeuAgendamento(MeuAgendamentoDto dto)
        {
            var agendamento = await _portalService.EditarMeuAgendamento(User.ObterClienteId(), User.ObterPetshopId(), dto);
            if (agendamento == null)
                return NotFound(new RespostaErro("Agendamento não encontrado."));

            return Ok(agendamento);
        }

        [HttpPatch("CancelarMeuAgendamento/{id}")]
        [Authorize(Policy = Politicas.Cliente)]
        public async Task<IActionResult> CancelarMeuAgendamento(int id)
        {
            var sucesso = await _portalService.CancelarMeuAgendamento(User.ObterClienteId(), User.ObterPetshopId(), id);
            if (!sucesso)
                return NotFound(new RespostaErro("Agendamento não encontrado."));

            return NoContent();
        }
    }
}
