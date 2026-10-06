using System.Data;
using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Services.Configuracao;
using PetShop.API.Utils;

namespace PetShop.API.Services.Agenda
{
    public class AgendaService : IAgendaService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguracaoService _configuracaoService;

        public AgendaService(AppDbContext context, IConfiguracaoService configuracaoService)
        {
            _context = context;
            _configuracaoService = configuracaoService;
        }

        public async Task<List<string>> HorariosDisponiveis(int administradorId, DateOnly data, int servicoId, int? agendamentoIgnoradoId)
        {
            var configuracao = await _configuracaoService.ObterModelo(administradorId);

            var servico = await _context.Servicos
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == servicoId && s.AdministradorId == administradorId && !s.Excluido && s.Ativo)
                ?? throw new RegraDeNegocioException("Serviço não encontrado ou inativo.");

            var livres = new List<string>();

            if (!configuracao.ObterDiasFuncionamento().Contains((int)data.DayOfWeek))
                return livres;

            var duracao = DuracaoDe(servico.DuracaoMinutos);
            var passo = Math.Max(configuracao.IntervaloEntreHorariosMinutos, 5);
            var inicioDoDia = data.ToDateTime(TimeOnly.MinValue);
            var ocupacao = await CarregarOcupacao(administradorId, inicioDoDia, inicioDoDia.AddDays(1), agendamentoIgnoradoId);
            var agora = DataHoraBrasil.Agora;

            for (var hora = configuracao.HoraAbertura;
                 hora + TimeSpan.FromMinutes(duracao) <= configuracao.HoraFechamento;
                 hora += TimeSpan.FromMinutes(passo))
            {
                var inicio = inicioDoDia + hora;
                var fim = inicio.AddMinutes(duracao);

                if (inicio <= agora)
                    continue;

                if (CaiNoIntervalo(inicio, fim, configuracao))
                    continue;

                var simultaneos = ocupacao.Count(o => o.Inicio < fim && o.Fim > inicio);
                if (simultaneos >= Math.Max(configuracao.CapacidadeSimultanea, 1))
                    continue;

                livres.Add(inicio.ToString("HH:mm"));
            }

            return livres;
        }

        public async Task<T> ExecutarComTravaDaAgenda<T>(int administradorId, Func<Task<T>> acao)
        {
            if (_context.Database.CurrentTransaction != null)
                throw new InvalidOperationException("A trava da agenda não pode ser aberta dentro de outra transação.");

            await using var transacao = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            // Trava exclusiva por loja, liberada automaticamente no commit/rollback.
            await _context.Database.ExecuteSqlRawAsync(@"
DECLARE @resultado int;
EXEC @resultado = sp_getapplock @Resource = {0}, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 15000;
IF @resultado < 0 THROW 50001, 'A agenda está ocupada. Tente novamente em instantes.', 1;",
                $"petshop-agenda-{administradorId}");

            var resultado = await acao();
            await transacao.CommitAsync();
            return resultado;
        }

        public async Task GarantirVaga(int administradorId, DateTime inicio, int duracaoMinutos, int? agendamentoIgnoradoId, ConfiguracaoLojaModel configuracao)
        {
            var fim = inicio.AddMinutes(DuracaoDe(duracaoMinutos));
            var ocupacao = await CarregarOcupacao(administradorId, inicio.Date, fim.Date.AddDays(1), agendamentoIgnoradoId);

            var simultaneos = ocupacao.Count(o => o.Inicio < fim && o.Fim > inicio);
            if (simultaneos >= Math.Max(configuracao.CapacidadeSimultanea, 1))
                throw new ConflitoException("Este horário acabou de ser ocupado. Escolha outro horário.");
        }

        public void ValidarHorarioDoPortal(DateTime inicio, int duracaoMinutos, ConfiguracaoLojaModel configuracao)
        {
            if (inicio == default)
                throw new RegraDeNegocioException("Informe a data e a hora do agendamento.");

            if (inicio <= DataHoraBrasil.Agora)
                throw new RegraDeNegocioException("Escolha uma data e hora futuras.");

            if (!configuracao.ObterDiasFuncionamento().Contains((int)inicio.DayOfWeek))
                throw new RegraDeNegocioException("O petshop não atende neste dia.");

            var fim = inicio.AddMinutes(DuracaoDe(duracaoMinutos));
            if (inicio.TimeOfDay < configuracao.HoraAbertura || fim.Date != inicio.Date || fim.TimeOfDay > configuracao.HoraFechamento)
                throw new RegraDeNegocioException("Horário fora do expediente do petshop.");

            if (CaiNoIntervalo(inicio, fim, configuracao))
                throw new RegraDeNegocioException("Horário dentro do intervalo do petshop.");
        }

        private async Task<List<(DateTime Inicio, DateTime Fim)>> CarregarOcupacao(int administradorId, DateTime de, DateTime ate, int? agendamentoIgnoradoId)
        {
            var desde = de.AddDays(-1);

            var agendamentos = await _context.Agendamentos
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(a => a.Pet.Cliente.AdministradorId == administradorId
                            && a.DataHora >= desde
                            && a.DataHora < ate
                            && (agendamentoIgnoradoId == null || a.Id != agendamentoIgnoradoId))
                .Select(a => new { a.DataHora, a.Status, Duracao = a.Servico.DuracaoMinutos })
                .ToListAsync();

            return agendamentos
                .Where(a => !StatusAgendamento.EhCancelado(a.Status))
                .Select(a => (a.DataHora, a.DataHora.AddMinutes(DuracaoDe(a.Duracao))))
                .ToList();
        }

        private static bool CaiNoIntervalo(DateTime inicio, DateTime fim, ConfiguracaoLojaModel configuracao)
        {
            if (!configuracao.InicioIntervalo.HasValue || !configuracao.FimIntervalo.HasValue)
                return false;

            var inicioIntervalo = inicio.Date + configuracao.InicioIntervalo.Value;
            var fimIntervalo = inicio.Date + configuracao.FimIntervalo.Value;
            return inicio < fimIntervalo && fim > inicioIntervalo;
        }

        private static int DuracaoDe(int duracaoMinutos) => Math.Max(duracaoMinutos, 1);
    }
}
