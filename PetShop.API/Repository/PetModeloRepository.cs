using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class PetModeloRepository : IPetModeloRepository
    {
        private readonly AppDbContext _context;

        public PetModeloRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(PetModelo pet)
        {
            _context.PetsModel.Add(pet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PetModelo pet)
        {
            _context.PetsModel.Remove(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PetModelo>> GetAllAsync()
        {
            return await _context.PetsModel
                .Include(p => p.Cliente)
                .ToListAsync();
        }

        public async Task<PetModelo?> GetByIdAsync(int id)
        {
            return await _context.PetsModel
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PetModelo>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.PetsModel
                .Include(p => p.Cliente)
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task UpdateAsync(PetModelo pet)
        {
            _context.PetsModel.Update(pet);
            await _context.SaveChangesAsync();
        }
    }
}
