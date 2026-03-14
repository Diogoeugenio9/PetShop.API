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

        public async Task AddAsync(PetModel pet)
        {
            _context.PetsModelo.Add(pet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PetModel pet)
        {
            _context.PetsModelo.Remove(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PetModel>> GetAllAsync()
        {
            return await _context.PetsModelo
                .Include(p => p.Cliente)
                .ToListAsync();
        }

        public async Task<PetModel?> GetByIdAsync(int id)
        {
            return await _context.PetsModelo
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PetModel>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.PetsModelo
                .Include(p => p.Cliente)
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task UpdateAsync(PetModel pet)
        {
            _context.PetsModelo.Update(pet);
            await _context.SaveChangesAsync();
        }
    }
}
