using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteModel>> GetAllAsync()
            => await _context.Clientes
                .Where(c => !c.Excluido)
                .ToListAsync();

        public async Task<ClienteModel> GetByIdAsync(int id)
            => await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id && !c.Excluido);

        public async Task<ClienteModel> GetByPetId(int petId)
            => await _context.PetsModelo
                .Where(p => p.Id == petId && !p.Excluido)
                .Select(p => p.Cliente)
                .FirstOrDefaultAsync();

        public async Task AddAsync(ClienteModel cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClienteModel cliente)
        {
            if (_context.Entry(cliente).State == EntityState.Detached)
                _context.Clientes.Update(cliente);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(ClienteModel cliente)
        {
            cliente.Excluido = true;

            var pets = await _context.PetsModelo
                .Where(p => p.ClienteId == cliente.Id && !p.Excluido)
                .ToListAsync();

            foreach (var pet in pets)
                pet.Excluido = true;

            await _context.SaveChangesAsync();
        }
    }
}
