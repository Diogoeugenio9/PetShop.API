namespace PetShop.API.Services.Assinatura
{
    public class RenovacaoAssinaturaWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RenovacaoAssinaturaWorker> _logger;
        private readonly TimeSpan _intervalo;

        public RenovacaoAssinaturaWorker(IServiceScopeFactory scopeFactory, ILogger<RenovacaoAssinaturaWorker> logger, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            var minutos = configuration.GetValue<int?>("Assinatura:IntervaloMinutos") ?? 60;
            _intervalo = TimeSpan.FromMinutes(Math.Max(minutos, 1));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_intervalo);

            do
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var servico = scope.ServiceProvider.GetRequiredService<IAssinaturaService>();
                    var encerradas = await servico.EncerrarRenovacoesCanceladas(stoppingToken);

                    if (encerradas > 0)
                        _logger.LogInformation("{Quantidade} renovação(ões) mensal(is) encerrada(s) no AbacatePay.", encerradas);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao encerrar renovações de assinatura canceladas.");
                }
            }
            while (await AguardarProximo(timer, stoppingToken));
        }

        private static async Task<bool> AguardarProximo(PeriodicTimer timer, CancellationToken stoppingToken)
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
