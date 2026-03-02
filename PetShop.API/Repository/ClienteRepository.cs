using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repositories.Cliente
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteModel>> GetAllAsync()
            => await _context.Clientes.ToListAsync();

        public async Task<ClienteModel?> GetByIdAsync(int id)
            => await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<ClienteModel?> GetByPetId(int petId)
            => await _context.PetsModelo
                .Include(p => p.Cliente)
                .Where(p => p.Id == petId)
                .Select(p => p.Cliente)
                .FirstOrDefaultAsync();

        public async Task AddAsync(ClienteModel cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClienteModel cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ClienteModel cliente)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
