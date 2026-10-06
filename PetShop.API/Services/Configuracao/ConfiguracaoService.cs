using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Configuracao;
using PetShop.API.Models;
using PetShop.API.Utils;

namespace PetShop.API.Services.Configuracao
{
    public class ConfiguracaoService : IConfiguracaoService
    {
        private readonly AppDbContext _context;

        public ConfiguracaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracaoLojaModel> ObterModelo(int administradorId)
        {
            var configuracao = await BuscarGravada(administradorId);
            return configuracao ?? ConfiguracaoLojaModel.Padrao(administradorId);
        }

        public async Task<ConfiguracaoLojaDto> Obter(int administradorId)
            => ParaDto(await ObterModelo(administradorId));

        public async Task<ConfiguracaoLojaDto> Salvar(int administradorId, ConfiguracaoLojaDto dto)
        {
            var abertura = LerHora(dto.HoraAbertura, "abertura", obrigatoria: true).Value;
            var fechamento = LerHora(dto.HoraFechamento, "fechamento", obrigatoria: true).Value;
            var inicioIntervalo = LerHora(dto.InicioIntervalo, "início do intervalo", obrigatoria: false);
            var fimIntervalo = LerHora(dto.FimIntervalo, "fim do intervalo", obrigatoria: false);

            if (fechamento <= abertura)
                throw new RegraDeNegocioException("A hora de fechamento deve ser depois da abertura.");

            if (inicioIntervalo.HasValue != fimIntervalo.HasValue)
                throw new RegraDeNegocioException("Informe o início e o fim do intervalo, ou deixe os dois vazios.");

            if (inicioIntervalo.HasValue &&
                (fimIntervalo <= inicioIntervalo || inicioIntervalo < abertura || fimIntervalo > fechamento))
                throw new RegraDeNegocioException("O intervalo deve estar dentro do expediente e terminar depois de começar.");

            var dias = (dto.DiasFuncionamento ?? new List<int>()).Where(d => d >= 0 && d <= 6).Distinct().OrderBy(d => d).ToList();
            if (dias.Count == 0)
                throw new RegraDeNegocioException("Informe ao menos um dia de funcionamento (0 = domingo ... 6 = sábado).");

            var configuracao = await BuscarGravada(administradorId);
            if (configuracao == null)
            {
                configuracao = ConfiguracaoLojaModel.Padrao(administradorId);
                _context.ConfiguracoesLoja.Add(configuracao);
            }

            configuracao.HoraAbertura = abertura;
            configuracao.HoraFechamento = fechamento;
            configuracao.InicioIntervalo = inicioIntervalo;
            configuracao.FimIntervalo = fimIntervalo;
            configuracao.IntervaloEntreHorariosMinutos = dto.IntervaloEntreHorariosMinutos;
            configuracao.CapacidadeSimultanea = dto.CapacidadeSimultanea;
            configuracao.AntecedenciaMinimaHoras = dto.AntecedenciaMinimaHoras;
            configuracao.DiasFuncionamento = string.Join(",", dias);
            configuracao.MetaAgendamentosMensal = dto.MetaAgendamentosMensal;

            await _context.SaveChangesAsync();
            return ParaDto(configuracao);
        }

        public async Task<MetaAgendamentosDto> ObterMeta(int administradorId)
        {
            var configuracao = await ObterModelo(administradorId);
            return new MetaAgendamentosDto { MetaMensal = configuracao.MetaAgendamentosMensal };
        }

        public async Task<MetaAgendamentosDto> DefinirMeta(int administradorId, MetaAgendamentosDto dto)
        {
            var configuracao = await BuscarGravada(administradorId);
            if (configuracao == null)
            {
                configuracao = ConfiguracaoLojaModel.Padrao(administradorId);
                _context.ConfiguracoesLoja.Add(configuracao);
            }

            configuracao.MetaAgendamentosMensal = dto.MetaMensal;
            await _context.SaveChangesAsync();

            return new MetaAgendamentosDto { MetaMensal = configuracao.MetaAgendamentosMensal };
        }

        private Task<ConfiguracaoLojaModel> BuscarGravada(int administradorId)
            => _context.ConfiguracoesLoja
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.AdministradorId == administradorId);

        private static TimeSpan? LerHora(string valor, string campo, bool obrigatoria)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                if (obrigatoria)
                    throw new RegraDeNegocioException($"Informe a hora de {campo}.");
                return null;
            }

            if (TimeSpan.TryParse(valor.Trim(), out var hora) && hora >= TimeSpan.Zero && hora < TimeSpan.FromDays(1))
                return hora;

            throw new RegraDeNegocioException($"Hora de {campo} inválida. Use o formato HH:mm.");
        }

        private static string FormatarHora(TimeSpan? hora)
            => hora.HasValue ? hora.Value.ToString(@"hh\:mm") : null;

        private static ConfiguracaoLojaDto ParaDto(ConfiguracaoLojaModel c) => new ConfiguracaoLojaDto
        {
            HoraAbertura = FormatarHora(c.HoraAbertura),
            HoraFechamento = FormatarHora(c.HoraFechamento),
            InicioIntervalo = FormatarHora(c.InicioIntervalo),
            FimIntervalo = FormatarHora(c.FimIntervalo),
            IntervaloEntreHorariosMinutos = c.IntervaloEntreHorariosMinutos,
            CapacidadeSimultanea = c.CapacidadeSimultanea,
            AntecedenciaMinimaHoras = c.AntecedenciaMinimaHoras,
            DiasFuncionamento = c.ObterDiasFuncionamento().OrderBy(d => d).ToList(),
            MetaAgendamentosMensal = c.MetaAgendamentosMensal,
            FusoHorario = "America/Sao_Paulo"
        };
    }
}
