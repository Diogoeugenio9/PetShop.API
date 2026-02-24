using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly AppDbContext _context;

        public ServicoRepository(AppDbContext context) => _context = context;

        public async Task<ServicoModel> AddAsync(ServicoModel servico)
        {
            _context.Servicos.Add(servico);
            await _context.SaveChangesAsync();
            return servico;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null) return false;

            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ServicoModel>> GetAllAsync()
        {
            return await _context.Servicos.ToListAsync();
        }

        public async Task<ServicoModel?> GetByIdAsync(int id)
        {
            return await _context.Servicos.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ServicoModel> UpdateAsync(ServicoModel servico)
        {
            _context.Servicos.Update(servico);
            await _context.SaveChangesAsync();
            return servico;
        }
    }
}
