using AutoMapper;
using PetShop.API.Dto.Produto;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

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

        public async Task<ProdutoModel?> BuscarProdutoPorId(int idProduto)
        {
            return await _repository.GetByIdAsync(idProduto);
        }

        public async Task<ProdutoModel> CriarProduto(ProdutoModeloDto produtoCriacaoDto)
        {
            var produto = _mapper.Map<ProdutoModel>(produtoCriacaoDto);
            produto.Ativo = true;

            await _repository.AddAsync(produto);
            return produto;
        }

        public async Task<ProdutoModel?> EditarProduto(ProdutoModeloDto produtoEdicaoDto)
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
            var produto = await _repository.GetByIdAsync(idProduto);

            if (produto == null)
                return false;

            return await _repository.DeleteAsync(idProduto);
        }

        public async Task<List<ProdutoModel>> ListarProdutos()
        {
            return await _repository.GetAllAsync();
        }
    }
}