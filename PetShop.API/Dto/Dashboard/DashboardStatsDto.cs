namespace PetShop.API.Dto.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalClientes { get; set; }
        public int TotalPets { get; set; }
        public int AgendamentosHoje { get; set; }
        public decimal ReceitaMes { get; set; }

        public GraficoDto Grafico { get; set; }
        public ServicosDistribuicaoDto ServicosDistribuicao { get; set; }
        public FaturamentoPorServicoDto FaturamentoPorServico { get; set; }
        public MetaMensalDto MetaMensal { get; set; }
    }
}