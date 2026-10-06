using PetShop.API.Dto.Configuracao;
using PetShop.API.Models;

namespace PetShop.API.Services.Configuracao
{
    public interface IConfiguracaoService
    {
        Task<ConfiguracaoLojaModel> ObterModelo(int administradorId);

        Task<ConfiguracaoLojaDto> Obter(int administradorId);
        Task<ConfiguracaoLojaDto> Salvar(int administradorId, ConfiguracaoLojaDto dto);

        Task<MetaAgendamentosDto> ObterMeta(int administradorId);
        Task<MetaAgendamentosDto> DefinirMeta(int administradorId, MetaAgendamentosDto dto);
    }
}
