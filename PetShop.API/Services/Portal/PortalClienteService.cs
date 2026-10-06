using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Agendamento;
using PetShop.API.Dto.Portal;
using PetShop.API.Dto.Vacina;
using PetShop.API.Models;
using PetShop.API.Services.Agenda;
using PetShop.API.Services.Configuracao;
using PetShop.API.Services.Vacina;
using PetShop.API.Utils;

namespace PetShop.API.Services.Portal
{
    public class PortalClienteService : IPortalClienteService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAgendaService _agendaService;
        private readonly IConfiguracaoService _configuracaoService;

        public PortalClienteService(
            AppDbContext context,
            IMapper mapper,
            IAgendaService agendaService,
            IConfiguracaoService configuracaoService)
        {
            _context = context;
            _mapper = mapper;
            _agendaService = agendaService;
            _configuracaoService = configuracaoService;
        }

        public async Task<PerfilClienteDto> ObterPerfil(int clienteId)
        {
            var cliente = await ObterClienteAtivo(clienteId);
            return ParaPerfil(cliente);
        }

        public async Task<PerfilClienteDto> EditarPerfil(int clienteId, PerfilClienteDto dto)
        {
            var cliente = await ObterClienteAtivo(clienteId);
            var email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();

            if (!string.Equals(email, cliente.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailEmUso = await _context.Clientes
                    .IgnoreQueryFilters()
                    .AnyAsync(c => c.Id != cliente.Id && c.Email == email && c.SenhaHash != null && !c.Excluido);

                if (emailEmUso)
                    throw new ConflitoException("Já existe uma conta com este e-mail.");
            }

            cliente.Nome = dto.Nome?.Trim();
            cliente.Sobrenome = dto.Sobrenome?.Trim();
            cliente.Email = email;
            cliente.Telefone = dto.Telefone?.Trim();

            var cpf = new string((dto.Cpf ?? string.Empty).Where(char.IsDigit).ToArray());
            if (cpf.Length > 0)
            {
                if (cpf.Length != 11)
                    throw new RegraDeNegocioException("CPF inválido. Informe os 11 dígitos.");
                cliente.Cpf = cpf;
            }

            if (dto.Endereco != null)
            {
                cliente.Cep = dto.Endereco.Cep?.Trim();
                cliente.Logradouro = dto.Endereco.Logradouro?.Trim();
                cliente.Numero = dto.Endereco.Numero?.Trim();
                cliente.Bairro = dto.Endereco.Bairro?.Trim();
                cliente.Cidade = dto.Endereco.Cidade?.Trim();
                cliente.Estado = dto.Endereco.Uf?.Trim();
            }

            await _context.SaveChangesAsync();
            return ParaPerfil(cliente);
        }

        public async Task<List<MeuPetDto>> ListarMeusPets(int clienteId)
        {
            await ObterClienteAtivo(clienteId);

            var pets = await _context.PetsModelo
                .AsNoTracking()
                .Where(p => p.ClienteId == clienteId && !p.Excluido)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            return pets.Select(ParaMeuPet).ToList();
        }

        public async Task<MeuPetDto> CriarMeuPet(int clienteId, MeuPetDto dto)
        {
            await ObterClienteAtivo(clienteId);

            var pet = new PetModelo
            {
                ClienteId = clienteId,
                DataCadastro = DataHoraBrasil.Agora,
                Ativo = true
            };
            PreencherPet(pet, dto);

            _context.PetsModelo.Add(pet);
            await _context.SaveChangesAsync();

            return ParaMeuPet(pet);
        }

        public async Task<MeuPetDto> EditarMeuPet(int clienteId, MeuPetDto dto)
        {
            await ObterClienteAtivo(clienteId);

            var pet = await BuscarMeuPet(clienteId, dto.Id);
            if (pet == null)
                return null;

            PreencherPet(pet, dto);
            await _context.SaveChangesAsync();

            return ParaMeuPet(pet);
        }

        public async Task<bool> ExcluirMeuPet(int clienteId, int petId)
        {
            await ObterClienteAtivo(clienteId);

            var pet = await BuscarMeuPet(clienteId, petId);
            if (pet == null)
                return false;

            var agora = DataHoraBrasil.Agora;
            var agendamentosFuturos = await _context.Agendamentos
                .Where(a => a.PetId == pet.Id && a.DataHora > agora)
                .Select(a => a.Status)
                .ToListAsync();

            if (agendamentosFuturos.Any(StatusAgendamento.EstaAberto))
                throw new RegraDeNegocioException("Este pet tem agendamentos futuros. Cancele-os antes de excluir o pet.");

            pet.Excluido = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ServicoModel>> ListarServicosAtivos()
        {
            return await _context.Servicos
                .AsNoTracking()
                .Where(s => s.Ativo && !s.Excluido)
                .OrderBy(s => s.Nome)
                .ToListAsync();
        }

        public async Task<List<AgendamentoDto>> ListarMeusAgendamentos(int clienteId)
        {
            await ObterClienteAtivo(clienteId);

            var agendamentos = await _context.Agendamentos
                .AsNoTracking()
                .Include(a => a.Pet)
                .Include(a => a.Servico)
                .Where(a => a.Pet.ClienteId == clienteId)
                .OrderByDescending(a => a.DataHora)
                .ToListAsync();

            return _mapper.Map<List<AgendamentoDto>>(agendamentos);
        }

        public async Task<AgendamentoDto> CriarMeuAgendamento(int clienteId, int petshopId, MeuAgendamentoDto dto)
        {
            await ObterClienteAtivo(clienteId);

            var pet = await BuscarMeuPet(clienteId, dto.PetId)
                ?? throw new RegraDeNegocioException("Pet não encontrado.");

            var servico = await BuscarServicoAtivo(dto.ServicoId);
            var configuracao = await _configuracaoService.ObterModelo(petshopId);

            _agendaService.ValidarHorarioDoPortal(dto.DataHora, servico.DuracaoMinutos, configuracao);

            var agendamento = new AgendamentoModel
            {
                DataHora = dto.DataHora,
                Status = StatusAgendamento.Pendente,
                Observacoes = dto.Observacoes?.Trim(),
                PetId = pet.Id,
                ServicoId = servico.Id
            };

            await _agendaService.ExecutarComTravaDaAgenda(petshopId, async () =>
            {
                await GarantirPetLivre(pet.Id, dto.DataHora, servico.DuracaoMinutos, null);
                await _agendaService.GarantirVaga(petshopId, dto.DataHora, servico.DuracaoMinutos, null, configuracao);

                _context.Agendamentos.Add(agendamento);
                await _context.SaveChangesAsync();
                return true;
            });

            agendamento.Pet = pet;
            agendamento.Servico = servico;
            return _mapper.Map<AgendamentoDto>(agendamento);
        }

        public async Task<AgendamentoDto> EditarMeuAgendamento(int clienteId, int petshopId, MeuAgendamentoDto dto)
        {
            await ObterClienteAtivo(clienteId);

            var agendamento = await BuscarMeuAgendamento(clienteId, dto.Id);
            if (agendamento == null)
                return null;

            if (!StatusAgendamento.EstaAberto(agendamento.Status))
                throw new RegraDeNegocioException("Este agendamento não pode mais ser alterado.");

            var configuracao = await _configuracaoService.ObterModelo(petshopId);
            ValidarAntecedencia(agendamento.DataHora, configuracao);

            var pet = await BuscarMeuPet(clienteId, dto.PetId)
                ?? throw new RegraDeNegocioException("Pet não encontrado.");

            var servico = await BuscarServicoAtivo(dto.ServicoId);

            _agendaService.ValidarHorarioDoPortal(dto.DataHora, servico.DuracaoMinutos, configuracao);

            await _agendaService.ExecutarComTravaDaAgenda(petshopId, async () =>
            {
                await GarantirPetLivre(pet.Id, dto.DataHora, servico.DuracaoMinutos, agendamento.Id);
                await _agendaService.GarantirVaga(petshopId, dto.DataHora, servico.DuracaoMinutos, agendamento.Id, configuracao);

                agendamento.DataHora = dto.DataHora;
                agendamento.PetId = pet.Id;
                agendamento.Pet = pet;
                agendamento.ServicoId = servico.Id;
                agendamento.Servico = servico;
                agendamento.Observacoes = dto.Observacoes?.Trim();
                agendamento.Status = StatusAgendamento.Pendente;

                await _context.SaveChangesAsync();
                return true;
            });

            return _mapper.Map<AgendamentoDto>(agendamento);
        }

        public async Task<bool> CancelarMeuAgendamento(int clienteId, int petshopId, int agendamentoId)
        {
            await ObterClienteAtivo(clienteId);

            var agendamento = await BuscarMeuAgendamento(clienteId, agendamentoId);
            if (agendamento == null)
                return false;

            if (StatusAgendamento.EhCancelado(agendamento.Status))
                return true;

            if (!StatusAgendamento.EstaAberto(agendamento.Status))
                throw new RegraDeNegocioException("Este agendamento não pode mais ser cancelado.");

            var configuracao = await _configuracaoService.ObterModelo(petshopId);
            ValidarAntecedencia(agendamento.DataHora, configuracao);

            agendamento.Status = StatusAgendamento.Cancelado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VacinaRespostaDto>> ListarMinhasVacinas(int clienteId)
        {
            await ObterClienteAtivo(clienteId);

            var vacinas = await _context.Vacinas
                .AsNoTracking()
                .Include(v => v.Pet).ThenInclude(p => p.Cliente)
                .Where(v => v.Pet.ClienteId == clienteId && !v.Pet.Excluido)
                .OrderByDescending(v => v.DataAplicacao)
                .ToListAsync();

            return vacinas.Select(VacinaService.ParaResposta).ToList();
        }

        private async Task<ClienteModel> ObterClienteAtivo(int clienteId)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == clienteId && !c.Excluido && c.SenhaHash != null);

            if (cliente == null || !cliente.Ativo)
                throw new AcessoNegadoException("Conta de cliente indisponível. Faça login novamente.");

            return cliente;
        }

        private Task<PetModelo> BuscarMeuPet(int clienteId, int petId)
            => _context.PetsModelo.FirstOrDefaultAsync(p => p.Id == petId && p.ClienteId == clienteId && !p.Excluido);

        private Task<AgendamentoModel> BuscarMeuAgendamento(int clienteId, int agendamentoId)
            => _context.Agendamentos
                .Include(a => a.Pet)
                .Include(a => a.Servico)
                .FirstOrDefaultAsync(a => a.Id == agendamentoId && a.Pet.ClienteId == clienteId);

        private async Task<ServicoModel> BuscarServicoAtivo(int servicoId)
        {
            return await _context.Servicos
                .FirstOrDefaultAsync(s => s.Id == servicoId && s.Ativo && !s.Excluido)
                ?? throw new RegraDeNegocioException("Serviço não encontrado ou indisponível.");
        }

        private async Task GarantirPetLivre(int petId, DateTime inicio, int duracaoMinutos, int? agendamentoIgnoradoId)
        {
            var fim = inicio.AddMinutes(Math.Max(duracaoMinutos, 1));
            var desde = inicio.AddDays(-1);

            var doPet = await _context.Agendamentos
                .AsNoTracking()
                .Where(a => a.PetId == petId
                            && a.DataHora >= desde
                            && a.DataHora < fim
                            && (agendamentoIgnoradoId == null || a.Id != agendamentoIgnoradoId))
                .Select(a => new { a.DataHora, a.Status, Duracao = a.Servico.DuracaoMinutos })
                .ToListAsync();

            var conflito = doPet.Any(a =>
                !StatusAgendamento.EhCancelado(a.Status) &&
                a.DataHora.AddMinutes(Math.Max(a.Duracao, 1)) > inicio);

            if (conflito)
                throw new ConflitoException("Este pet já tem um agendamento nesse horário.");
        }

        private static void ValidarAntecedencia(DateTime dataHoraAgendada, ConfiguracaoLojaModel configuracao)
        {
            var antecedencia = TimeSpan.FromHours(configuracao.AntecedenciaMinimaHoras);
            if (dataHoraAgendada - DataHoraBrasil.Agora < antecedencia)
                throw new RegraDeNegocioException(
                    $"Alterações e cancelamentos precisam ser feitos com pelo menos {configuracao.AntecedenciaMinimaHoras} hora(s) de antecedência.");
        }

        private static void PreencherPet(PetModelo pet, MeuPetDto dto)
        {
            pet.Nome = dto.Nome?.Trim();
            pet.Especie = dto.Especie?.Trim();
            pet.Raca = dto.Raca?.Trim();
            pet.Idade = dto.Idade;
            pet.Sexo = dto.Sexo?.Trim();
            pet.Peso = dto.Peso;
            pet.Observacoes = dto.Observacoes?.Trim();
        }

        private static MeuPetDto ParaMeuPet(PetModelo pet) => new MeuPetDto
        {
            Id = pet.Id,
            Nome = pet.Nome,
            Especie = pet.Especie,
            Raca = pet.Raca,
            Idade = pet.Idade,
            Sexo = pet.Sexo,
            Peso = pet.Peso,
            Observacoes = pet.Observacoes
        };

        private static PerfilClienteDto ParaPerfil(ClienteModel c) => new PerfilClienteDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Sobrenome = c.Sobrenome,
            Cpf = c.Cpf,
            Email = c.Email,
            Telefone = c.Telefone,
            Endereco = new EnderecoDto
            {
                Cep = c.Cep,
                Logradouro = c.Logradouro,
                Numero = c.Numero,
                Bairro = c.Bairro,
                Cidade = c.Cidade,
                Uf = c.Estado
            }
        };
    }
}
