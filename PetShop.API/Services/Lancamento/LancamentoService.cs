using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Lancamento;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;
using PetShop.API.Utils;

namespace PetShop.API.Services.Lancamento
{
    public class LancamentoService : ILancamentoService
    {
        private readonly ILancamentoRepository _repository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public LancamentoService(ILancamentoRepository repository, IMapper mapper, AppDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<LancamentoModel> BuscarLancamentoPorId(int idLancamento)
        {
            return await _repository.GetByIdAsync(idLancamento);
        }

        public async Task<LancamentoModel> CriarLancamento(LancamentoDto lancamentoCriacaoDto)
        {
            var tipo = ValidarTipo(lancamentoCriacaoDto.Tipo);
            await ValidarVinculos(lancamentoCriacaoDto, idIgnorado: null);

            if (lancamentoCriacaoDto.AgendamentoId.HasValue)
            {
                var existente = await _context.Lancamentos
                    .FirstOrDefaultAsync(l => l.AgendamentoId == lancamentoCriacaoDto.AgendamentoId);
                if (existente != null)
                    return existente;
            }

            var lancamento = _mapper.Map<LancamentoModel>(lancamentoCriacaoDto);
            lancamento.Id = 0;
            lancamento.Tipo = tipo;

            await _repository.AddAsync(lancamento);

            return lancamento;
        }

        public async Task<LancamentoModel> EditarLancamento(LancamentoDto lancamentoEdicaoDto)
        {
            var tipo = ValidarTipo(lancamentoEdicaoDto.Tipo);

            var lancamento = await _repository.GetByIdAsync(lancamentoEdicaoDto.Id);

            if (lancamento == null)
                return null;

            await ValidarVinculos(lancamentoEdicaoDto, idIgnorado: lancamento.Id);

            var agendamentoAnterior = lancamento.AgendamentoId;
            var servicoAnterior = lancamento.ServicoId;

            _mapper.Map(lancamentoEdicaoDto, lancamento);
            lancamento.Tipo = tipo;

            lancamento.AgendamentoId ??= agendamentoAnterior;
            lancamento.ServicoId ??= servicoAnterior;

            await _repository.UpdateAsync(lancamento);

            return lancamento;
        }

        public async Task<bool> ExcluirLancamento(int idLancamento)
        {
            return await _repository.DeleteAsync(idLancamento);
        }

        public async Task<List<LancamentoModel>> ListarLancamentos()
        {
            return await _repository.GetAllAsync();
        }

        private async Task ValidarVinculos(LancamentoDto dto, int? idIgnorado)
        {
            if (dto.AgendamentoId.HasValue)
            {
                var agendamentoDaLoja = await _context.Agendamentos.AnyAsync(a => a.Id == dto.AgendamentoId);
                if (!agendamentoDaLoja)
                    throw new RegraDeNegocioException("Agendamento não encontrado.");

                if (idIgnorado.HasValue)
                {
                    var usadoEmOutro = await _context.Lancamentos
                        .AnyAsync(l => l.AgendamentoId == dto.AgendamentoId && l.Id != idIgnorado);
                    if (usadoEmOutro)
                        throw new ConflitoException("Já existe um lançamento para este agendamento.");
                }
            }

            if (dto.ServicoId.HasValue)
            {
                var servicoDaLoja = await _context.Servicos.AnyAsync(s => s.Id == dto.ServicoId);
                if (!servicoDaLoja)
                    throw new RegraDeNegocioException("Serviço não encontrado.");
            }
        }

        private static string ValidarTipo(string tipo)
        {
            var normalizado = TextoNormalizado.Normalizar(tipo);

            if (normalizado != "receita" && normalizado != "despesa")
                throw new RegraDeNegocioException("Tipo de lançamento inválido. Use \"receita\" ou \"despesa\".");

            return normalizado;
        }
    }
}
