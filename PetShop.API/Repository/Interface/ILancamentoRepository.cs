using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface ILancamentoRepository
    {
        Task<List<LancamentoModel>> GetAllAsync();
        Task<LancamentoModel?> GetByIdAsync(int id);
        Task<LancamentoModel> AddAsync(LancamentoModel lancamento);
        Task<LancamentoModel> UpdateAsync(LancamentoModel lancamento);
        Task<bool> DeleteAsync(int id);
    }
}