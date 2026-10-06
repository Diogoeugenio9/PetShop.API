using AutoMapper;
using PetShop.API.Dto.Agendamento;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;
using PetShop.API.Services.Agenda;
using PetShop.API.Services.Configuracao;
using PetShop.API.Utils;

namespace PetShop.API.Services.Agendamento
{
    public class AgendamentoService : IAgendamentoService
    {
        private const string StatusInicial = StatusAgendamento.Pendente;

        private readonly IAgendamentoRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAgendaService _agendaService;
        private readonly IConfiguracaoService _configuracaoService;
        private readonly IConclusaoAgendamentoService _conclusaoService;

        public AgendamentoService(
            IAgendamentoRepository repository,
            IMapper mapper,
            IAgendaService agendaService,
            IConfiguracaoService configuracaoService,
            IConclusaoAgendamentoService conclusaoService)
        {
            _repository = repository;
            _mapper = mapper;
            _agendaService = agendaService;
            _configuracaoService = configuracaoService;
            _conclusaoService = conclusaoService;
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

            var statusInformado = string.IsNullOrWhiteSpace(dto.Status) ? StatusInicial : dto.Status.Trim();
            var noPassado = dto.DataHora <= DataHoraBrasil.Agora;
            var cancelado = StatusAgendamento.EhCancelado(statusInformado);

            var concluir = !cancelado && (noPassado || StatusAgendamento.EhConcluido(statusInformado));

            if (concluir && !noPassado)
                throw new RegraDeNegocioException(_conclusaoService.MensagemDe(ResultadoConclusao.HorarioAindaNaoChegou));

            var agendamento = new AgendamentoModel
            {
                DataHora = dto.DataHora,
                Status = concluir ? StatusInicial : statusInformado,
                Observacoes = dto.Observacoes?.Trim(),
                PetId = pet.Id,
                ServicoId = servico.Id
            };

            if (noPassado || cancelado)
            {
                await _repository.AddAsync(agendamento);
            }
            else
            {
                var administradorId = servico.AdministradorId;
                var configuracao = await _configuracaoService.ObterModelo(administradorId);

                await _agendaService.ExecutarComTravaDaAgenda(administradorId, async () =>
                {
                    await _agendaService.GarantirVaga(administradorId, dto.DataHora, servico.DuracaoMinutos, null, configuracao);
                    await _repository.AddAsync(agendamento);
                    return true;
                });
            }

            if (concluir)
                await ConcluirOuFalhar(agendamento);

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

            var novoStatus = string.IsNullOrWhiteSpace(agendamentoEdicaoDto.Status)
                ? agendamento.Status
                : agendamentoEdicaoDto.Status.Trim();

            var concluir = StatusAgendamento.EhConcluido(novoStatus) && !StatusAgendamento.EhConcluido(agendamento.Status);
            var futuro = agendamentoEdicaoDto.DataHora > DataHoraBrasil.Agora;

            if (concluir && futuro)
                throw new RegraDeNegocioException(_conclusaoService.MensagemDe(ResultadoConclusao.HorarioAindaNaoChegou));

            var mudouHorario = agendamento.DataHora != agendamentoEdicaoDto.DataHora || agendamento.ServicoId != servico.Id;

            agendamento.DataHora = agendamentoEdicaoDto.DataHora;
            agendamento.PetId = pet.Id;
            agendamento.Pet = pet;
            agendamento.ServicoId = servico.Id;
            agendamento.Servico = servico;
            agendamento.Observacoes = agendamentoEdicaoDto.Observacoes?.Trim();

            if (!concluir)
                agendamento.Status = novoStatus;

            if (mudouHorario && futuro && !StatusAgendamento.EhCancelado(agendamento.Status))
            {
                var administradorId = servico.AdministradorId;
                var configuracao = await _configuracaoService.ObterModelo(administradorId);

                await _agendaService.ExecutarComTravaDaAgenda(administradorId, async () =>
                {
                    await _agendaService.GarantirVaga(administradorId, agendamento.DataHora, servico.DuracaoMinutos, agendamento.Id, configuracao);
                    await _repository.UpdateAsync(agendamento);
                    return true;
                });
            }
            else
            {
                await _repository.UpdateAsync(agendamento);
            }

            if (concluir)
                await ConcluirOuFalhar(agendamento);

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

            if (StatusAgendamento.EhConcluido(novoStatus))
            {
                await ConcluirOuFalhar(agendamento);
                return true;
            }

            agendamento.Status = novoStatus.Trim();
            await _repository.UpdateAsync(agendamento);
            return true;
        }

        public async Task<bool> ExcluirAgendamento(int idAgendamento)
        {
            return await _repository.DeleteAsync(idAgendamento);
        }

        private async Task ConcluirOuFalhar(AgendamentoModel agendamento)
        {
            var resultado = await _conclusaoService.Concluir(agendamento.Id);

            if (resultado != ResultadoConclusao.Concluido && resultado != ResultadoConclusao.JaEstavaConcluido)
                throw new RegraDeNegocioException(_conclusaoService.MensagemDe(resultado));

            await _repository.MarcarComoConcluidoEmMemoria(agendamento);
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
