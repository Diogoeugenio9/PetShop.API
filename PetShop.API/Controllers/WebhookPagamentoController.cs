using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Services.Pagamento;

namespace PetShop.API.Controllers
{
    [Route("api/integracoes/abacatepay/webhook")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WebhookPagamentoController : ControllerBase
    {
        private readonly IWebhookPagamentoService _webhookService;

        public WebhookPagamentoController(IWebhookPagamentoService webhookService)
        {
            _webhookService = webhookService;
        }

        [HttpPost]
        public async Task<IActionResult> Receber([FromQuery] string webhookSecret)
        {
            using var leitor = new StreamReader(Request.Body);
            var corpo = await leitor.ReadToEndAsync();

            var assinatura = Request.Headers["X-Webhook-Signature"].ToString();
            if (!_webhookService.OrigemValida(corpo, assinatura, webhookSecret))
                return Unauthorized();

            await _webhookService.Processar(corpo);
            return Ok();
        }
    }
}
