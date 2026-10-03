using AutoMapper;
using PetShop.API.Dto.Agendamento;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;
using PetShop.API.Utils;

namespace PetShop.API.Services.Agendamento
{
    public class AgendamentoService : IAgendamentoService
    {
        private const string StatusInicial = "Pendente";

        private readonly IAgendamentoRepository _repository;
        private readonly IMapper _mapper;

        public AgendamentoService(IAgendamentoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AgendamentoDto>> ListarAgendamentos()
        {
            var agendamentos = await _repository.GetAllAsync();
            return _mapper.Map<List<AgendamentoDto>>(agendamentos);
        }

        public async Task<AgendamentoDto> BuscarAgendamentoPorId(int idAgendamento)
        {
            var agendamento = await _repository.GetByIdAsync(idAgendamento);
            return _mapper.Map<AgendamentoDto>(agendamento);
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
            var (pet, servico) = await ValidarAgendamento(dto.DataHora, dto.PetId, dto.ServicoId, idIgnorado: null);

            var agendamento = new AgendamentoModel
            {
                DataHora = dto.DataHora,
                Status = StatusInicial,
                PetId = pet.Id,
                ServicoId = servico.Id
            };

            await _repository.AddAsync(agendamento);

            agendamento.Pet = pet;
            agendamento.Servico = servico;
            return _mapper.Map<AgendamentoDto>(agendamento);
        }

        public async Task<AgendamentoDto> EditarAgendamento(AgendamentoDto agendamentoEdicaoDto)
        {
            var agendamento = await _repository.GetByIdAsync(agendamentoEdicaoDto.Id);
            if (agendamento == null) return null;

            var (pet, servico) = await ValidarAgendamento(
                agendamentoEdicaoDto.DataHora,
                agendamentoEdicaoDto.PetId,
                agendamentoEdicaoDto.ServicoId,
                idIgnorado: agendamento.Id);

            agendamento.DataHora = agendamentoEdicaoDto.DataHora;
            agendamento.PetId = pet.Id;
            agendamento.Pet = pet;
            agendamento.ServicoId = servico.Id;
            agendamento.Servico = servico;

            if (!string.IsNullOrWhiteSpace(agendamentoEdicaoDto.Status))
                agendamento.Status = agendamentoEdicaoDto.Status.Trim();

            await _repository.UpdateAsync(agendamento);

            return _mapper.Map<AgendamentoDto>(agendamento);
        }

        public async Task<bool> AlterarStatus(int idAgendamento, string novoStatus)
        {
            if (string.IsNullOrWhiteSpace(novoStatus))
                throw new RegraDeNegocioException("Informe o novo status.");

            if (novoStatus.Trim().Length > 30)
                throw new RegraDeNegocioException("O status deve ter no máximo 30 caracteres.");

            var agendamento = await _repository.GetByIdAsync(idAgendamento);
            if (agendamento == null) return false;

            agendamento.Status = novoStatus.Trim();
            await _repository.UpdateAsync(agendamento);
            return true;
        }

        public async Task<bool> ExcluirAgendamento(int idAgendamento)
        {
            return await _repository.DeleteAsync(idAgendamento);
        }

        private async Task<(PetModelo pet, ServicoModel servico)> ValidarAgendamento(
            DateTime dataHora, int petId, int servicoId, int? idIgnorado)
        {
            if (dataHora == default)
                throw new RegraDeNegocioException("Informe a data e a hora do agendamento.");

            var pet = await _repository.GetPetByIdAsync(petId)
                ?? throw new RegraDeNegocioException("Pet não encontrado.");

            var servico = await _repository.GetServicoByIdAsync(servicoId)
                ?? throw new RegraDeNegocioException("Serviço não encontrado.");

            var inicio = dataHora;
            var fim = dataHora.AddMinutes(Math.Max(servico.DuracaoMinutos, 1));

            var agendamentosDoPet = await _repository.GetByPetIdAsync(petId);

            var conflito = agendamentosDoPet.Any(a =>
                a.Id != idIgnorado &&
                !StatusAgendamento.EhCancelado(a.Status) &&
                a.DataHora < fim &&
                a.DataHora.AddMinutes(Math.Max(a.Servico?.DuracaoMinutos ?? 0, 1)) > inicio);

            if (conflito)
                throw new RegraDeNegocioException("Este pet já tem um agendamento nesse horário.");

            return (pet, servico);
        }
    }
}
