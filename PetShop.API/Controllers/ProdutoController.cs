using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Produto;
using PetShop.API.Models;
using PetShop.API.Services.Produto;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet("ListarProdutos")]
        public async Task<ActionResult<List<ProdutoModel>>> ListarProdutos()
        {
            var produtos = await _produtoService.ListarProdutos();
            return Ok(produtos);
        }

        [HttpGet("BuscarPorId/{id}")]
        public async Task<ActionResult<ProdutoModel>> BuscarPorId(int id)
        {
            var produto = await _produtoService.BuscarProdutoPorId(id);

            if (produto == null)
                return NotFound("Produto não encontrado");

            return Ok(produto);
        }

        [HttpPost("CriarProduto")]
        public async Task<ActionResult<ProdutoModel>> CriarProduto(ProdutoModeloDto produtoCriacaoDto)
        {
            var produto = await _produtoService.CriarProduto(produtoCriacaoDto);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = produto.Id },
                produto
            );
        }

        [HttpPut("EditarProduto")]
        public async Task<ActionResult<ProdutoModel>> EditarProduto(ProdutoModeloDto produtoEdicaoDto)
        {
            var produto = await _produtoService.EditarProduto(produtoEdicaoDto);

            if (produto == null)
                return NotFound("Produto não encontrado");

            return Ok(produto);
        }

        [HttpDelete("ExcluirProduto/{id}")]
        public async Task<IActionResult> ExcluirProduto(int id)
        {
            var sucesso = await _produtoService.ExcluirProduto(id);

            if (!sucesso)
                return NotFound("Produto não encontrado");

            return NoContent();
        }
    }
}