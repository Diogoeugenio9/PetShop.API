using PetShop.API.Dto.Agendamento;
using PetShop.API.Dto.Portal;
using PetShop.API.Dto.Vacina;
using PetShop.API.Models;

namespace PetShop.API.Services.Portal
{
    public interface IPortalClienteService
    {
        Task<PerfilClienteDto> ObterPerfil(int clienteId);
        Task<PerfilClienteDto> EditarPerfil(int clienteId, PerfilClienteDto dto);

        Task<List<MeuPetDto>> ListarMeusPets(int clienteId);
        Task<MeuPetDto> CriarMeuPet(int clienteId, MeuPetDto dto);
        Task<MeuPetDto> EditarMeuPet(int clienteId, MeuPetDto dto);
        Task<bool> ExcluirMeuPet(int clienteId, int petId);

        Task<List<ServicoModel>> ListarServicosAtivos();

        Task<List<AgendamentoDto>> ListarMeusAgendamentos(int clienteId);
        Task<AgendamentoDto> CriarMeuAgendamento(int clienteId, int petshopId, MeuAgendamentoDto dto);
        Task<AgendamentoDto> EditarMeuAgendamento(int clienteId, int petshopId, MeuAgendamentoDto dto);
        Task<bool> CancelarMeuAgendamento(int clienteId, int petshopId, int agendamentoId);

        Task<List<VacinaRespostaDto>> ListarMinhasVacinas(int clienteId);
    }
}
