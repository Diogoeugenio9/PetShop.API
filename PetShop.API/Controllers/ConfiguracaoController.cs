using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Configuracao;
using PetShop.API.Services.Configuracao;
using PetShop.API.Utils;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Politicas.Administrador)]
    public class ConfiguracaoController : ControllerBase
    {
        private readonly IConfiguracaoService _configuracaoService;

        public ConfiguracaoController(IConfiguracaoService configuracaoService)
        {
            _configuracaoService = configuracaoService;
        }

        [HttpGet("ObterConfiguracao")]
        public async Task<ActionResult<ConfiguracaoLojaDto>> ObterConfiguracao()
        {
            return Ok(await _configuracaoService.Obter(User.ObterAdministradorId()));
        }

        [HttpPut("EditarConfiguracao")]
        public async Task<ActionResult<ConfiguracaoLojaDto>> EditarConfiguracao(ConfiguracaoLojaDto dto)
        {
            return Ok(await _configuracaoService.Salvar(User.ObterAdministradorId(), dto));
        }

        [HttpGet("ObterMeta")]
        public async Task<ActionResult<MetaAgendamentosDto>> ObterMeta()
        {
            return Ok(await _configuracaoService.ObterMeta(User.ObterAdministradorId()));
        }

        [HttpPut("DefinirMeta")]
        public async Task<ActionResult<MetaAgendamentosDto>> DefinirMeta(MetaAgendamentosDto dto)
        {
            return Ok(await _configuracaoService.DefinirMeta(User.ObterAdministradorId(), dto));
        }
    }
}
