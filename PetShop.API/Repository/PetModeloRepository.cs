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
            _context.PetsModelo.Add(pet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PetModelo pet)
        {
            pet.Excluido = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<PetModelo>> GetAllAsync()
        {
            return await _context.PetsModelo
                .Include(p => p.Cliente)
                .Where(p => !p.Excluido)
                .ToListAsync();
        }

        public async Task<PetModelo> GetByIdAsync(int id)
        {
            return await _context.PetsModelo
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido);
        }

        public async Task<List<PetModelo>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.PetsModelo
                .Include(p => p.Cliente)
                .Where(p => p.ClienteId == clienteId && !p.Excluido)
                .ToListAsync();
        }

        public async Task UpdateAsync(PetModelo pet)
        {
            if (_context.Entry(pet).State == EntityState.Detached)
                _context.PetsModelo.Update(pet);

            await _context.SaveChangesAsync();
        }
    }
}
