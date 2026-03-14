using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Repository
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly AppDbContext _context;

        public AgendamentoRepository(AppDbContext context) => _context = context;

        public async Task<List<AgendamentoModel>> GetAllAsync()
            => await _context.Agendamentos.Include(a => a.Pet).Include(a => a.Servico).ToListAsync();

        public async Task<AgendamentoModel?> GetByIdAsync(int id)
            => await _context.Agendamentos.Include(a => a.Pet).Include(a => a.Servico).FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<AgendamentoModel>> GetByPetIdAsync(int petId)
            => await _context.Agendamentos.Where(a => a.PetId == petId).ToListAsync();

        public async Task<List<AgendamentoModel>> GetByClienteIdAsync(int clienteId)
            => await _context.Agendamentos.Include(a => a.Pet)
                                          .Where(a => a.Pet.ClienteId == clienteId)
                                          .ToListAsync();

        public async Task<AgendamentoModel> AddAsync(AgendamentoModel agendamento)
        {
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();
            return agendamento;
        }

        public async Task<AgendamentoModel> UpdateAsync(AgendamentoModel agendamento)
        {
            _context.Agendamentos.Update(agendamento);
            await _context.SaveChangesAsync();
            return agendamento;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null) return false;

            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PetModel?> GetPetByIdAsync(int petId)
        {
            return await _context.PetsModelo.FirstOrDefaultAsync(p => p.Id == petId);
        }

        public async Task<ServicoModel?> GetServicoByIdAsync(int servicoId)
        {
            return await _context.Servicos.FirstOrDefaultAsync(s => s.Id == servicoId);
        }

    }


}
