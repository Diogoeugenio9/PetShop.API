using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Models;
using PetShop.API.Utils;

namespace PetShop.API.Services.Agenda
{
    public class ConclusaoAgendamentoService : IConclusaoAgendamentoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConclusaoAgendamentoService> _logger;

        public ConclusaoAgendamentoService(AppDbContext context, ILogger<ConclusaoAgendamentoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResultadoConclusao> Concluir(int agendamentoId)
        {
            var agora = DataHoraBrasil.Agora;
            var transacaoPropria = _context.Database.CurrentTransaction == null;
            var transacao = transacaoPropria ? await _context.Database.BeginTransactionAsync() : null;

            try
            {
                // UPDATE condicional: relê o estado no próprio banco. Só conclui se ainda estiver
                // aberto (não cancelado/reagendado/concluído) e se o horário (atual) já chegou.
                var linhas = await _context.Agendamentos
                    .IgnoreQueryFilters()
                    .Where(a => a.Id == agendamentoId
                                && a.DataHora <= agora
                                && (a.Status == null || a.Status == "" || a.Status == StatusAgendamento.Pendente || a.Status == StatusAgendamento.Confirmado || a.Status == "Agendado"))
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, StatusAgendamento.Concluido));

                var agendamento = await _context.Agendamentos
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Include(a => a.Pet).ThenInclude(p => p.Cliente)
                    .Include(a => a.Servico)
                    .FirstOrDefaultAsync(a => a.Id == agendamentoId);

                ResultadoConclusao resultado;
                if (agendamento == null)
                    resultado = ResultadoConclusao.NaoEncontrado;
                else if (linhas == 1)
                    resultado = ResultadoConclusao.Concluido;
                else if (StatusAgendamento.EhConcluido(agendamento.Status))
                    resultado = ResultadoConclusao.JaEstavaConcluido;
                else if (StatusAgendamento.EhCancelado(agendamento.Status))
                    resultado = ResultadoConclusao.Cancelado;
                else if (agendamento.DataHora > agora)
                    resultado = ResultadoConclusao.HorarioAindaNaoChegou;
                else
                    resultado = ResultadoConclusao.StatusNaoPermite;

                if (resultado == ResultadoConclusao.Concluido || resultado == ResultadoConclusao.JaEstavaConcluido)
                    await GarantirReceita(agendamento);

                if (transacao != null)
                    await transacao.CommitAsync();

                return resultado;
            }
            catch
            {
                if (transacao != null)
                    await transacao.RollbackAsync();
                throw;
            }
            finally
            {
                if (transacao != null)
                    await transacao.DisposeAsync();
            }
        }

        public async Task<int> ConcluirVencidos(CancellationToken cancellationToken)
        {
            var agora = DataHoraBrasil.Agora;

            var ids = await _context.Agendamentos
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(a => a.DataHora <= agora
                            && (a.Status == null || a.Status == "" || a.Status == StatusAgendamento.Pendente || a.Status == StatusAgendamento.Confirmado || a.Status == "Agendado"))
                .OrderBy(a => a.DataHora)
                .Select(a => a.Id)
                .Take(200)
                .ToListAsync(cancellationToken);

            var concluidos = 0;
            foreach (var id in ids)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    if (await Concluir(id) == ResultadoConclusao.Concluido)
                        concluidos++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao concluir automaticamente o agendamento {AgendamentoId}", id);
                    _context.ChangeTracker.Clear();
                }
            }

            return concluidos;
        }

        public string MensagemDe(ResultadoConclusao resultado) => resultado switch
        {
            ResultadoConclusao.NaoEncontrado => "Agendamento não encontrado.",
            ResultadoConclusao.Cancelado => "Não é possível concluir um agendamento cancelado.",
            ResultadoConclusao.HorarioAindaNaoChegou => "Não é possível concluir um agendamento antes do horário marcado.",
            ResultadoConclusao.StatusNaoPermite => "O status atual do agendamento não permite concluí-lo.",
            _ => null
        };

        private async Task GarantirReceita(AgendamentoModel agendamento)
        {
            var jaExiste = await _context.Lancamentos
                .IgnoreQueryFilters()
                .AnyAsync(l => l.AgendamentoId == agendamento.Id);

            if (jaExiste || agendamento.Servico == null || agendamento.Pet?.Cliente == null)
                return;

            var receita = new LancamentoModel
            {
                Descricao = $"{agendamento.Servico.Nome} - {agendamento.Pet.Nome}",
                Valor = agendamento.Servico.Preco,
                Tipo = "receita",
                Data = agendamento.DataHora.Date,
                Categoria = "Servicos",
                FormaPagamento = null,
                Observacoes = "Receita gerada automaticamente na conclusão do agendamento.",
                AgendamentoId = agendamento.Id,
                ServicoId = agendamento.ServicoId,
                AdministradorId = agendamento.Pet.Cliente.AdministradorId
            };

            _context.Lancamentos.Add(receita);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (EhViolacaoDeUnicidade(ex))
            {
                _context.Entry(receita).State = EntityState.Detached;
            }
        }

        public static bool EhViolacaoDeUnicidade(DbUpdateException ex)
            => ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
    }
}
