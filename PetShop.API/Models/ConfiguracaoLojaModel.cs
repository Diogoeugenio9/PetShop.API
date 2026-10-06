namespace PetShop.API.Models
{
    public class ConfiguracaoLojaModel : IPertenceALoja
    {
        public int Id { get; set; }
        public int AdministradorId { get; set; }

        public TimeSpan HoraAbertura { get; set; }
        public TimeSpan HoraFechamento { get; set; }
        public TimeSpan? InicioIntervalo { get; set; }
        public TimeSpan? FimIntervalo { get; set; }

        public int IntervaloEntreHorariosMinutos { get; set; }

        public int CapacidadeSimultanea { get; set; }

        public int AntecedenciaMinimaHoras { get; set; }

        public string DiasFuncionamento { get; set; }

        public int? MetaAgendamentosMensal { get; set; }

        public static ConfiguracaoLojaModel Padrao(int administradorId) => new ConfiguracaoLojaModel
        {
            AdministradorId = administradorId,
            HoraAbertura = new TimeSpan(8, 0, 0),
            HoraFechamento = new TimeSpan(18, 0, 0),
            InicioIntervalo = new TimeSpan(12, 0, 0),
            FimIntervalo = new TimeSpan(13, 0, 0),
            IntervaloEntreHorariosMinutos = 30,
            CapacidadeSimultanea = 1,
            AntecedenciaMinimaHoras = 2,
            DiasFuncionamento = "1,2,3,4,5,6",
            MetaAgendamentosMensal = null
        };

        public HashSet<int> ObterDiasFuncionamento()
        {
            var dias = new HashSet<int>();
            foreach (var parte in (DiasFuncionamento ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(parte, out var dia) && dia >= 0 && dia <= 6)
                    dias.Add(dia);
            }
            return dias;
        }
    }
}
