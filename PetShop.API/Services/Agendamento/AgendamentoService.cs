using AutoMapper;
using PetShop.API.Dto.Agendamento;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Services.Agendamento
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _repository;
        private readonly IMapper _mapper;

        public AgendamentoService(IAgendamentoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Listar todos os agendamentos
        public async Task<List<AgendamentoDto>> ListarAgendamentos()
        {
            var agendamentos = await _repository.GetAllAsync();
            return _mapper.Map<List<AgendamentoDto>>(agendamentos);
        }

        public async Task<AgendamentoDto?> BuscarAgendamentoPorId(int idAgendamento)
        {
            var agendamento = await _repository.GetByIdAsync(idAgendamento);
            return _mapper.Map<AgendamentoDto?>(agendamento);
        }


        public async Task<List<AgendamentoDto>> BuscarAgendamentosPorPet(int petId)
        {
            var agendamentos = await _repository.GetByPetIdAsync(petId);
            return _mapper.Map<List<AgendamentoDto>>(agendamentos);
        }

        public async Task<List<AgendamentoDto>> BuscarAgendamentosPorCliente(int clienteId)
        {
            var agendamentos = await _repository.GetByClienteIdAsync(clienteId);
            return _mapper.Map<List<AgendamentoDto>>(agendamentos);
        }

        public async Task<AgendamentoDto> CriarAgendamento(AgendamentoCreateDto dto)
        {
            var agendamento = new AgendamentoModel
            {
                DataHora = dto.DataHora,
                Status = "Pendente",
                PetId = dto.PetId,
                ServicoId = dto.ServicoId
            };

            await _repository.AddAsync(agendamento);

            var pet = await _repository.GetPetByIdAsync(dto.PetId);
            var servico = await _repository.GetServicoByIdAsync(dto.ServicoId);

            return new AgendamentoDto
            {
                Id = agendamento.Id,
                DataHora = agendamento.DataHora,
                Status = agendamento.Status,
                PetId = agendamento.PetId,
                ServicoId = agendamento.ServicoId,
                NomePet = pet?.Nome,
                NomeServico = servico?.Nome
            };
        }


        public async Task<AgendamentoDto?> EditarAgendamento(AgendamentoDto agendamentoEdicaoDto)
        {
            var agendamento = await _repository.GetByIdAsync(agendamentoEdicaoDto.Id);
            if (agendamento == null) return null;

            _mapper.Map(agendamentoEdicaoDto, agendamento);
            await _repository.UpdateAsync(agendamento);

            return _mapper.Map<AgendamentoDto>(agendamento);
        }


        // Alterar status
        public async Task<bool> AlterarStatus(int idAgendamento, string novoStatus)
        {
            var agendamento = await _repository.GetByIdAsync(idAgendamento);
            if (agendamento == null) return false;

            agendamento.Status = novoStatus;
            await _repository.UpdateAsync(agendamento);
            return true;
        }

        // Excluir agendamento
        public async Task<bool> ExcluirAgendamento(int idAgendamento)
        {
            var agendamento = await _repository.GetByIdAsync(idAgendamento);
            if (agendamento == null) return false;

            return await _repository.DeleteAsync(idAgendamento);
        }
    }
}
