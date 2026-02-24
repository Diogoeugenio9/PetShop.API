using AutoMapper;
using PetShop.API.Dto.Servico;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;
using PetShop.API.Services.Servico.PetShop.API.Services.Servico;

namespace PetShop.API.Services.Servico
{
    public class ServicoService : IServicoService
    {
        private readonly IServicoRepository _repository;
        private readonly IMapper _mapper;

        public ServicoService(IServicoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ServicoModel?> BuscarServicoPorId(int idServico)
        {
            return await _repository.GetByIdAsync(idServico);
        }

        public async Task<ServicoModel> CriarServico(ServicoDto servicoCriacaoDto)
        {
            var servico = _mapper.Map<ServicoModel>(servicoCriacaoDto);
            servico.Ativo = true; // sempre ativo ao criar

            await _repository.AddAsync(servico);
            return servico;
        }

        public async Task<ServicoModel?> EditarServico(ServicoDto servicoEdicaoDto)
        {
            var servico = await _repository.GetByIdAsync(servicoEdicaoDto.Id);

            if (servico == null)
                return null;

            _mapper.Map(servicoEdicaoDto, servico);
            await _repository.UpdateAsync(servico);

            return servico;
        }

        public async Task<bool> ExcluirServico(int idServico)
        {
            var servico = await _repository.GetByIdAsync(idServico);

            if (servico == null)
                return false;

            return await _repository.DeleteAsync(idServico);
        }

        public async Task<List<ServicoModel>> ListarServicos()
        {
            return await _repository.GetAllAsync();
        }
    }
}
