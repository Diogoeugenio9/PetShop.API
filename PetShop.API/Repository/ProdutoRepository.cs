using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context) => _context = context;

        public async Task<ProdutoModel> AddAsync(ProdutoModel produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var produto = await GetByIdAsync(id);
            if (produto == null) return false;

            produto.Excluido = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProdutoModel>> GetAllAsync()
        {
            return await _context.Produtos
                .Where(p => !p.Excluido)
                .ToListAsync();
        }

        public async Task<ProdutoModel> GetByIdAsync(int id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido);
        }

        public async Task<ProdutoModel> UpdateAsync(ProdutoModel produto)
        {
            if (_context.Entry(produto).State == EntityState.Detached)
                _context.Produtos.Update(produto);

            await _context.SaveChangesAsync();
            return produto;
        }
    }
}
