namespace PetShop.API.Services.Agenda
{
    public class ConclusaoAutomaticaWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ConclusaoAutomaticaWorker> _logger;
        private readonly TimeSpan _intervalo;

        public ConclusaoAutomaticaWorker(IServiceScopeFactory scopeFactory, ILogger<ConclusaoAutomaticaWorker> logger, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            var segundos = configuration.GetValue<int?>("ConclusaoAutomatica:IntervaloSegundos") ?? 60;
            _intervalo = TimeSpan.FromSeconds(Math.Max(segundos, 15));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_intervalo);

            do
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var servico = scope.ServiceProvider.GetRequiredService<IConclusaoAgendamentoService>();
                    var concluidos = await servico.ConcluirVencidos(stoppingToken);

                    if (concluidos > 0)
                        _logger.LogInformation("{Quantidade} agendamento(s) concluído(s) automaticamente.", concluidos);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro na conclusão automática de agendamentos.");
                }
            }
            while (await WaitNext(timer, stoppingToken));
        }

        private static async Task<bool> WaitNext(PeriodicTimer timer, CancellationToken stoppingToken)
        {
            try
            {
                return await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}
