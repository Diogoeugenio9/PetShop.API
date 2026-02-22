using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PetModel>> ListarTodos()
        {
            return await _context.Pets
                .Include(p => p.Cliente)
                .ToListAsync();
        }

        public async Task<PetModel?> BuscarPorId(int idPet)
        {
            return await _context.Pets
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == idPet);
        }

        public async Task<List<PetModel>> BuscarPorIdCliente(int idCliente)
        {
            return await _context.Pets
                .Where(p => p.ClienteId == idCliente)
                .ToListAsync();
        }

        public async Task Criar(PetModel pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
        }

        public async Task Atualizar(PetModel pet)
        {
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task Remover(PetModel pet)
        {
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PetModel>> GetAll()
        {
            return await _context.Pets
                .Include(p => p.Cliente)
                .ToListAsync();
        }

        public async Task<PetModel?> GetById(int id)
        {
            return await _context.Pets
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PetModel>> GetByClienteId(int clienteId)
        {
            return await _context.Pets
                .Where(p => p.ClienteId == clienteId)
                .Include(p => p.Cliente)
                .ToListAsync();
        }

        public async Task Add(PetModel pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
        }

        public async Task Update(PetModel pet)
        {
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(PetModel pet)
        {
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }

    }
}
