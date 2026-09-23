using PetShop.API.Models;

namespace PetShop.API.Repository.Interface
{
    public interface IProdutoRepository
    {
        Task<List<ProdutoModel>> GetAllAsync();
        Task<ProdutoModel?> GetByIdAsync(int id);

        Task<ProdutoModel> AddAsync(ProdutoModel produto);
        Task<ProdutoModel> UpdateAsync(ProdutoModel produto);
        Task<bool> DeleteAsync(int id);
    }
}