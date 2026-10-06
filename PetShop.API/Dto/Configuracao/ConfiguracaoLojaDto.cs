using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Configuracao
{
    public class ConfiguracaoLojaDto
    {
        [Required(ErrorMessage = "Informe a hora de abertura.")]
        public string HoraAbertura { get; set; }

        [Required(ErrorMessage = "Informe a hora de fechamento.")]
        public string HoraFechamento { get; set; }

        public string InicioIntervalo { get; set; }

        public string FimIntervalo { get; set; }

        [Range(5, 240, ErrorMessage = "O intervalo entre horários deve ficar entre 5 e 240 minutos.")]
        public int IntervaloEntreHorariosMinutos { get; set; }

        [Range(1, 50, ErrorMessage = "A capacidade simultânea deve ficar entre 1 e 50.")]
        public int CapacidadeSimultanea { get; set; }

        [Range(0, 168, ErrorMessage = "A antecedência mínima deve ficar entre 0 e 168 horas.")]
        public int AntecedenciaMinimaHoras { get; set; }

        public List<int> DiasFuncionamento { get; set; } = new List<int>();

        [Range(1, 100000, ErrorMessage = "A meta deve ser maior que zero.")]
        public int? MetaAgendamentosMensal { get; set; }

        public string FusoHorario { get; set; } = "America/Sao_Paulo";
    }

    public class MetaAgendamentosDto
    {
        [Range(1, 100000, ErrorMessage = "A meta deve ser maior que zero.")]
        public int? MetaMensal { get; set; }
    }
}
