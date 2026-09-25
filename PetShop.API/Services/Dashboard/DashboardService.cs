using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Dashboard;

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
            var hoje = DateTime.Today;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var totalClientes = await _context.Clientes.CountAsync();
            var totalPets = await _context.PetsModelo.CountAsync();

            var agendamentosHoje = await _context.Agendamentos
                .CountAsync(a => a.DataHora.Date == hoje);

            var receitaMes = await _context.Lancamentos
                .Where(l =>
                    l.Tipo == "receita" &&
                    l.Data >= inicioMes &&
                    l.Data < inicioMes.AddMonths(1))
                .SumAsync(l => (decimal?)l.Valor) ?? 0;

            var grafico = await ObterGrafico();
            var servicosDistribuicao = await ObterServicosDistribuicao();
            var faturamentoPorServico = await ObterFaturamentoPorServico();

            return new DashboardStatsDto
            {
                TotalClientes = totalClientes,
                TotalPets = totalPets,
                AgendamentosHoje = agendamentosHoje,
                ReceitaMes = receitaMes,
                Grafico = grafico,
                ServicosDistribuicao = servicosDistribuicao,
                FaturamentoPorServico = faturamentoPorServico,
                MetaMensal = new MetaMensalDto
                {
                    TresMeses = new MetaDto(),
                    SeisMeses = new MetaDto(),
                    UmAno = new MetaDto()
                }
            };
        }

        private async Task<GraficoDto> ObterGrafico()
        {
            var hoje = DateTime.Today;

            var inicioTresMeses = hoje.AddMonths(-2);
            var inicioSeisMeses = hoje.AddMonths(-5);
            var inicioUmAno = hoje.AddMonths(-11);

            return new GraficoDto
            {
                TresMeses = await ObterDadosGrafico(inicioTresMeses, hoje),
                SeisMeses = await ObterDadosGrafico(inicioSeisMeses, hoje),
                UmAno = await ObterDadosGrafico(inicioUmAno, hoje)
            };
        }

        private async Task<List<GraficoMesDto>> ObterDadosGrafico(
            DateTime inicio,
            DateTime fim)
        {
            var lancamentos = await _context.Lancamentos
                .Where(l => l.Data >= inicio && l.Data <= fim)
                .ToListAsync();

            return lancamentos
                .GroupBy(l => new { l.Data.Year, l.Data.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new GraficoMesDto
                {
                    Mes = new DateTime(g.Key.Year, g.Key.Month, 1)
                        .ToString("MMM"),
                    Receitas = g
                        .Where(l => l.Tipo == "receita")
                        .Sum(l => l.Valor),
                    Despesas = g
                        .Where(l => l.Tipo == "despesa")
                        .Sum(l => l.Valor)
                })
                .ToList();
        }

        private async Task<ServicosDistribuicaoDto> ObterServicosDistribuicao()
        {
            var hoje = DateTime.Today;

            return new ServicosDistribuicaoDto
            {
                TresMeses = await ObterDistribuicao(hoje.AddMonths(-2), hoje),
                SeisMeses = await ObterDistribuicao(hoje.AddMonths(-5), hoje),
                UmAno = await ObterDistribuicao(hoje.AddMonths(-11), hoje)
            };
        }

        private async Task<List<ServicoDistribuicaoItemDto>> ObterDistribuicao(
            DateTime inicio,
            DateTime fim)
        {
            return await _context.Agendamentos
                .Where(a => a.DataHora.Date >= inicio && a.DataHora.Date <= fim)
                .Include(a => a.Servico)
                .GroupBy(a => a.Servico.Nome)
                .Select(g => new ServicoDistribuicaoItemDto
                {
                    Nome = g.Key,
                    Valor = g.Count()
                })
                .ToListAsync();
        }

        private async Task<FaturamentoPorServicoDto> ObterFaturamentoPorServico()
        {
            var hoje = DateTime.Today;

            return new FaturamentoPorServicoDto
            {
                TresMeses = await ObterFaturamento(hoje.AddMonths(-2), hoje),
                SeisMeses = await ObterFaturamento(hoje.AddMonths(-5), hoje),
                UmAno = await ObterFaturamento(hoje.AddMonths(-11), hoje)
            };
        }

        private async Task<List<FaturamentoServicoItemDto>> ObterFaturamento(
            DateTime inicio,
            DateTime fim)
        {
            return await _context.Agendamentos
                .Where(a => a.DataHora.Date >= inicio && a.DataHora.Date <= fim)
                .Include(a => a.Servico)
                .GroupBy(a => new
                {
                    a.ServicoId,
                    a.Servico.Nome
                })
                .Select(g => new FaturamentoServicoItemDto
                {
                    Servico = g.Key.Nome,
                    Faturamento = g.Sum(a => a.Servico.Preco)
                })
                .ToListAsync();
        }
    }
}