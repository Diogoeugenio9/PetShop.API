using AutoMapper;
using PetShop.API.Dto.Lancamento;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

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

        public async Task<LancamentoModel?> BuscarLancamentoPorId(int idLancamento)
        {
            return await _repository.GetByIdAsync(idLancamento);
        }

        public async Task<LancamentoModel> CriarLancamento(LancamentoDto lancamentoCriacaoDto)
        {
            var lancamento = _mapper.Map<LancamentoModel>(lancamentoCriacaoDto);

            await _repository.AddAsync(lancamento);

            return lancamento;
        }

        public async Task<LancamentoModel?> EditarLancamento(LancamentoDto lancamentoEdicaoDto)
        {
            var lancamento = await _repository.GetByIdAsync(lancamentoEdicaoDto.Id);

            if (lancamento == null)
                return null;

            _mapper.Map(lancamentoEdicaoDto, lancamento);

            await _repository.UpdateAsync(lancamento);

            return lancamento;
        }

        public async Task<bool> ExcluirLancamento(int idLancamento)
        {
            var lancamento = await _repository.GetByIdAsync(idLancamento);

            if (lancamento == null)
                return false;

            return await _repository.DeleteAsync(idLancamento);
        }

        public async Task<List<LancamentoModel>> ListarLancamentos()
        {
            return await _repository.GetAllAsync();
        }
    }
}