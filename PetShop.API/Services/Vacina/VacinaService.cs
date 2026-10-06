using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Vacina;
using PetShop.API.Models;
using PetShop.API.Utils;

namespace PetShop.API.Services.Vacina
{
    public class VacinaService : IVacinaService
    {
        private readonly AppDbContext _context;

        public VacinaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VacinaRespostaDto>> ListarVacinas()
        {
            var vacinas = await Consulta()
                .OrderByDescending(v => v.DataAplicacao)
                .ToListAsync();

            return vacinas.Select(ParaResposta).ToList();
        }

        public async Task<VacinaRespostaDto> BuscarVacinaPorId(int id)
        {
            var vacina = await Consulta().FirstOrDefaultAsync(v => v.Id == id);
            return vacina == null ? null : ParaResposta(vacina);
        }

        public async Task<VacinaRespostaDto> CriarVacina(VacinaDto dto)
        {
            var pet = await BuscarPetDaLoja(dto.PetId);
            Validar(dto);

            var vacina = new VacinaModel { PetId = pet.Id };
            Preencher(vacina, dto);

            _context.Vacinas.Add(vacina);
            await _context.SaveChangesAsync();

            vacina.Pet = pet;
            return ParaResposta(vacina);
        }

        public async Task<VacinaRespostaDto> EditarVacina(VacinaDto dto)
        {
            var vacina = await Consulta(rastrear: true).FirstOrDefaultAsync(v => v.Id == dto.Id);
            if (vacina == null)
                return null;

            if (dto.PetId != vacina.PetId)
            {
                var pet = await BuscarPetDaLoja(dto.PetId);
                vacina.PetId = pet.Id;
                vacina.Pet = pet;
            }

            Validar(dto);
            Preencher(vacina, dto);

            await _context.SaveChangesAsync();
            return ParaResposta(vacina);
        }

        public async Task<bool> ExcluirVacina(int id)
        {
            var vacina = await _context.Vacinas.FirstOrDefaultAsync(v => v.Id == id);
            if (vacina == null)
                return false;

            _context.Vacinas.Remove(vacina);
            await _context.SaveChangesAsync();
            return true;
        }

        public static VacinaRespostaDto ParaResposta(VacinaModel v) => new VacinaRespostaDto
        {
            Id = v.Id,
            PetId = v.PetId,
            PetNome = v.Pet?.Nome,
            ClienteNome = v.Pet?.Cliente == null ? null : $"{v.Pet.Cliente.Nome} {v.Pet.Cliente.Sobrenome}".Trim(),
            NomeVacina = v.NomeVacina,
            Fabricante = v.Fabricante,
            Lote = v.Lote,
            DataAplicacao = v.DataAplicacao,
            DataProximaDose = v.DataProximaDose,
            DoseUnica = v.DoseUnica,
            Observacoes = v.Observacoes
        };

        private IQueryable<VacinaModel> Consulta(bool rastrear = false)
        {
            var consulta = _context.Vacinas
                .Include(v => v.Pet).ThenInclude(p => p.Cliente)
                .Where(v => !v.Pet.Excluido);

            return rastrear ? consulta : consulta.AsNoTracking();
        }

        private async Task<PetModelo> BuscarPetDaLoja(int petId)
        {
            return await _context.PetsModelo
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == petId && !p.Excluido)
                ?? throw new RegraDeNegocioException("Pet não encontrado.");
        }

        private static void Validar(VacinaDto dto)
        {
            if (dto.DataAplicacao == default)
                throw new RegraDeNegocioException("Informe a data de aplicação.");

            if (dto.DataProximaDose.HasValue && dto.DataProximaDose.Value < dto.DataAplicacao)
                throw new RegraDeNegocioException("A próxima dose não pode ser antes da aplicação.");
        }

        private static void Preencher(VacinaModel vacina, VacinaDto dto)
        {
            vacina.NomeVacina = dto.NomeVacina?.Trim();
            vacina.Fabricante = dto.Fabricante?.Trim();
            vacina.Lote = dto.Lote?.Trim();
            vacina.DataAplicacao = dto.DataAplicacao;
            vacina.DoseUnica = dto.DoseUnica;
            vacina.DataProximaDose = dto.DoseUnica ? null : dto.DataProximaDose;
            vacina.Observacoes = dto.Observacoes?.Trim();
        }
    }
}
