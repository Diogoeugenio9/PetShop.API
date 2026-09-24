using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class LancamentoRepository : ILancamentoRepository
    {
        private readonly AppDbContext _context;

        public LancamentoRepository(AppDbContext context) => _context = context;

        public async Task<LancamentoModel> AddAsync(LancamentoModel lancamento)
        {
            _context.Lancamentos.Add(lancamento);
            await _context.SaveChangesAsync();
            return lancamento;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lancamento = await _context.Lancamentos.FindAsync(id);

            if (lancamento == null)
                return false;

            _context.Lancamentos.Remove(lancamento);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<LancamentoModel>> GetAllAsync()
        {
            return await _context.Lancamentos.ToListAsync();
        }

        public async Task<LancamentoModel?> GetByIdAsync(int id)
        {
            return await _context.Lancamentos.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<LancamentoModel> UpdateAsync(LancamentoModel lancamento)
        {
            _context.Lancamentos.Update(lancamento);
            await _context.SaveChangesAsync();

            return lancamento;
        }
    }
}