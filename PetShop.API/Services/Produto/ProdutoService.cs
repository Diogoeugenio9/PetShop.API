using AutoMapper;
using PetShop.API.Dto.Produto;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;
using PetShop.API.Utils;

namespace PetShop.API.Services.Produto
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repository;
        private readonly IMapper _mapper;

        public ProdutoService(IProdutoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProdutoModel> BuscarProdutoPorId(int idProduto)
        {
            return await _repository.GetByIdAsync(idProduto);
        }

        public async Task<ProdutoModel> CriarProduto(ProdutoModeloDto produtoCriacaoDto)
        {
            var produto = _mapper.Map<ProdutoModel>(produtoCriacaoDto);
            produto.Id = 0;
            produto.Ativo = true;

            await _repository.AddAsync(produto);
            return produto;
        }

        public async Task<ProdutoModel> EditarProduto(ProdutoModeloDto produtoEdicaoDto)
        {
            var produto = await _repository.GetByIdAsync(produtoEdicaoDto.Id);

            if (produto == null)
                return null;

            _mapper.Map(produtoEdicaoDto, produto);
            await _repository.UpdateAsync(produto);

            return produto;
        }

        public async Task<bool> ExcluirProduto(int idProduto)
        {
            return await _repository.DeleteAsync(idProduto);
        }

        public async Task<List<ProdutoModel>> ListarProdutos()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ProdutoModel> MovimentarProduto(MovimentarProdutoDto movimentarProdutoDto)
        {
            var tipo = TextoNormalizado.Normalizar(movimentarProdutoDto.Tipo);

            if (tipo != "entrada" && tipo != "saida")
                throw new RegraDeNegocioException("Tipo de movimentação inválido. Use \"entrada\" ou \"saida\".");

            var produto = await _repository.GetByIdAsync(movimentarProdutoDto.ProdutoId);

            if (produto == null)
                return null;

            if (tipo == "entrada")
            {
                produto.Quantidade += movimentarProdutoDto.Quantidade;
            }
            else
            {
                if (movimentarProdutoDto.Quantidade > produto.Quantidade)
                    throw new RegraDeNegocioException(
                        $"Estoque insuficiente. Disponível: {produto.Quantidade} {produto.Unidade}".Trim() + ".");

                produto.Quantidade -= movimentarProdutoDto.Quantidade;
            }

            await _repository.UpdateAsync(produto);

            return produto;
        }
    }
}
