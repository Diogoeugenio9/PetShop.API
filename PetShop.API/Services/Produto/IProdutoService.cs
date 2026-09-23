using PetShop.API.Dto.Produto;
using PetShop.API.Models;

namespace PetShop.API.Services.Produto
{
    public interface IProdutoService
    {
        Task<List<ProdutoModel>> ListarProdutos();
        Task<ProdutoModel?> BuscarProdutoPorId(int idProduto);

        Task<ProdutoModel> CriarProduto(ProdutoModeloDto produtoCriacaoDto);
        Task<ProdutoModel?> EditarProduto(ProdutoModeloDto produtoEdicaoDto);
        Task<bool> ExcluirProduto(int idProduto);
    }
}