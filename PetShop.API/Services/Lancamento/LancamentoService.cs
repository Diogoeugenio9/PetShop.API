using AutoMapper;
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

        public LancamentoService(ILancamentoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<LancamentoModel> BuscarLancamentoPorId(int idLancamento)
        {
            return await _repository.GetByIdAsync(idLancamento);
        }

        public async Task<LancamentoModel> CriarLancamento(LancamentoDto lancamentoCriacaoDto)
        {
            var lancamento = _mapper.Map<LancamentoModel>(lancamentoCriacaoDto);
            lancamento.Id = 0;
            lancamento.Tipo = ValidarTipo(lancamentoCriacaoDto.Tipo);

            await _repository.AddAsync(lancamento);

            return lancamento;
        }

        public async Task<LancamentoModel> EditarLancamento(LancamentoDto lancamentoEdicaoDto)
        {
            var tipo = ValidarTipo(lancamentoEdicaoDto.Tipo);

            var lancamento = await _repository.GetByIdAsync(lancamentoEdicaoDto.Id);

            if (lancamento == null)
                return null;

            _mapper.Map(lancamentoEdicaoDto, lancamento);
            lancamento.Tipo = tipo;

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

        private static string ValidarTipo(string tipo)
        {
            var normalizado = TextoNormalizado.Normalizar(tipo);

            if (normalizado != "receita" && normalizado != "despesa")
                throw new RegraDeNegocioException("Tipo de lançamento inválido. Use \"receita\" ou \"despesa\".");

            return normalizado;
        }
    }
}
