using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Dashboard;
using PetShop.API.Utils;

namespace PetShop.API.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> ObterStats()
        {
            var hoje = DataHoraBrasil.Hoje;
            var amanha = hoje.AddDays(1);
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var inicioUmAno = inicioMes.AddMonths(-11);

            var totalClientes = await _context.Clientes.CountAsync(c => !c.Excluido);
            var totalPets = await _context.PetsModelo.CountAsync(p => !p.Excluido);

            var lancamentos = await _context.Lancamentos
                .Where(l => l.Data >= inicioUmAno && l.Data < amanha)
                .Select(l => new LancamentoResumo(l.Data, l.Tipo, l.Valor))
                .ToListAsync();

            var agendamentos = await _context.Agendamentos
                .Where(a => a.DataHora >= inicioUmAno && a.DataHora < amanha)
                .Select(a => new AgendamentoResumo(a.DataHora, a.Status, a.Servico.Nome, a.Servico.Preco))
                .ToListAsync();

            agendamentos = agendamentos
                .Where(a => !StatusAgendamento.EhCancelado(a.Status))
                .ToList();

            var agendamentosHoje = await _context.Agendamentos
                .Where(a => a.DataHora >= hoje && a.DataHora < amanha)
                .Select(a => a.Status)
                .ToListAsync();

            var receitaMes = lancamentos
                .Where(l => l.Data >= inicioMes && EhTipo(l.Tipo, "receita"))
                .Sum(l => l.Valor);

            return new DashboardStatsDto
            {
                TotalClientes = totalClientes,
                TotalPets = totalPets,
                AgendamentosHoje = agendamentosHoje.Count(s => !StatusAgendamento.EhCancelado(s)),
                ReceitaMes = receitaMes,
                Grafico = new GraficoDto
                {
                    TresMeses = MontarGrafico(lancamentos, inicioMes, 3),
                    SeisMeses = MontarGrafico(lancamentos, inicioMes, 6),
                    UmAno = MontarGrafico(lancamentos, inicioMes, 12)
                },
                ServicosDistribuicao = new ServicosDistribuicaoDto
                {
                    TresMeses = MontarDistribuicao(agendamentos, inicioMes.AddMonths(-2)),
                    SeisMeses = MontarDistribuicao(agendamentos, inicioMes.AddMonths(-5)),
                    UmAno = MontarDistribuicao(agendamentos, inicioUmAno)
                },
                FaturamentoPorServico = new FaturamentoPorServicoDto
                {
                    TresMeses = MontarFaturamento(agendamentos, inicioMes.AddMonths(-2)),
                    SeisMeses = MontarFaturamento(agendamentos, inicioMes.AddMonths(-5)),
                    UmAno = MontarFaturamento(agendamentos, inicioUmAno)
                },
                MetaMensal = new MetaMensalDto
                {
                    TresMeses = new MetaDto(),
                    SeisMeses = new MetaDto(),
                    UmAno = new MetaDto()
                }
            };
        }

        private static List<GraficoMesDto> MontarGrafico(List<LancamentoResumo> lancamentos, DateTime inicioMesAtual, int quantidadeMeses)
        {
            var resultado = new List<GraficoMesDto>();

            for (var i = quantidadeMeses - 1; i >= 0; i--)
            {
                var inicio = inicioMesAtual.AddMonths(-i);
                var fim = inicio.AddMonths(1);
                var doMes = lancamentos.Where(l => l.Data >= inicio && l.Data < fim).ToList();

                resultado.Add(new GraficoMesDto
                {
                    Mes = NomeDoMes(inicio),
                    Receitas = doMes.Where(l => EhTipo(l.Tipo, "receita")).Sum(l => l.Valor),
                    Despesas = doMes.Where(l => EhTipo(l.Tipo, "despesa")).Sum(l => l.Valor)
                });
            }

            return resultado;
        }

        private static List<ServicoDistribuicaoItemDto> MontarDistribuicao(List<AgendamentoResumo> agendamentos, DateTime inicio)
        {
            return agendamentos
                .Where(a => a.DataHora >= inicio)
                .GroupBy(a => a.Servico)
                .Select(g => new ServicoDistribuicaoItemDto
                {
                    Nome = g.Key,
                    Valor = g.Count()
                })
                .OrderByDescending(i => i.Valor)
                .ToList();
        }

        private static List<FaturamentoServicoItemDto> MontarFaturamento(List<AgendamentoResumo> agendamentos, DateTime inicio)
        {
            return agendamentos
                .Where(a => a.DataHora >= inicio)
                .GroupBy(a => a.Servico)
                .Select(g => new FaturamentoServicoItemDto
                {
                    Servico = g.Key,
                    Faturamento = g.Sum(a => a.Preco)
                })
                .OrderByDescending(i => i.Faturamento)
                .ToList();
        }

        private static string NomeDoMes(DateTime data)
        {
            var nome = data.ToString("MMM", DataHoraBrasil.Cultura).TrimEnd('.');
            return nome.Length == 0 ? nome : char.ToUpper(nome[0], DataHoraBrasil.Cultura) + nome.Substring(1);
        }

        private static bool EhTipo(string tipo, string esperado)
            => string.Equals(tipo?.Trim(), esperado, StringComparison.OrdinalIgnoreCase);

        private record LancamentoResumo(DateTime Data, string Tipo, decimal Valor);

        private record AgendamentoResumo(DateTime DataHora, string Status, string Servico, decimal Preco);
    }
}
