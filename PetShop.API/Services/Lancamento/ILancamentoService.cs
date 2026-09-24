using PetShop.API.Dto.Lancamento;
using PetShop.API.Models;

namespace PetShop.API.Services.Lancamento
{
    public interface ILancamentoService
    {
        Task<List<LancamentoModel>> ListarLancamentos();
        Task<LancamentoModel?> BuscarLancamentoPorId(int idLancamento);
        Task<LancamentoModel> CriarLancamento(LancamentoDto lancamentoCriacaoDto);
        Task<LancamentoModel?> EditarLancamento(LancamentoDto lancamentoEdicaoDto);
        Task<bool> ExcluirLancamento(int idLancamento);
    }
}