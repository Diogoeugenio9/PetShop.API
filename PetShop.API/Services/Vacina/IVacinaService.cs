using PetShop.API.Dto.Vacina;

namespace PetShop.API.Services.Vacina
{
    public interface IVacinaService
    {
        Task<List<VacinaRespostaDto>> ListarVacinas();
        Task<VacinaRespostaDto> BuscarVacinaPorId(int id);
        Task<VacinaRespostaDto> CriarVacina(VacinaDto dto);
        Task<VacinaRespostaDto> EditarVacina(VacinaDto dto);
        Task<bool> ExcluirVacina(int id);
    }
}
