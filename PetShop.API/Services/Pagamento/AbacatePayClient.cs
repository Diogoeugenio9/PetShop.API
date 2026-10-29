using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using PetShop.API.Utils;

namespace PetShop.API.Services.Pagamento
{
    public class AbacatePayClient : IAbacatePayClient
    {
        private static readonly JsonSerializerOptions SemNulos = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        private readonly HttpClient _http;
        private readonly ILogger<AbacatePayClient> _logger;

        public AbacatePayClient(HttpClient http, IConfiguration configuration, ILogger<AbacatePayClient> logger)
        {
            _http = http;
            _logger = logger;

            var chave = configuration["AbacatePay:ApiKey"];
            if (string.IsNullOrWhiteSpace(chave))
                throw new InvalidOperationException("A chave AbacatePay:ApiKey não foi configurada.");

            _http.BaseAddress = new Uri(configuration["AbacatePay:Url"] ?? "https://api.abacatepay.com/v2/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", chave);
        }

        public async Task<CheckoutGateway> CriarCheckout(string produtoId, string referencia, string urlConclusao, string urlRetorno)
        {
            var data = await Enviar(HttpMethod.Post, "checkouts/create", new
            {
                items = new[] { new { id = produtoId, quantity = 1 } },
                externalId = referencia,
                completionUrl = urlConclusao,
                returnUrl = urlRetorno
            });

            return LerCheckout(data);
        }

        public async Task<CheckoutGateway> CriarCheckoutAssinatura(string produtoId, string referencia, string urlConclusao, string urlRetorno)
        {
            var data = await Enviar(HttpMethod.Post, "subscriptions/create", new
            {
                items = new[] { new { id = produtoId, quantity = 1 } },
                externalId = referencia,
                completionUrl = urlConclusao,
                returnUrl = urlRetorno,
                methods = new[] { "CARD" }
            });

            return LerCheckout(data);
        }

        public async Task<CheckoutGateway> BuscarCheckout(string checkoutId)
        {
            var data = await Enviar(HttpMethod.Get, $"checkouts/get?id={Uri.EscapeDataString(checkoutId)}");
            return LerCheckout(data);
        }

        public async Task<AssinaturaGateway> BuscarAssinaturaDoCheckout(string checkoutId)
        {
            var data = await Enviar(HttpMethod.Get, $"subscriptions/list?checkoutId={Uri.EscapeDataString(checkoutId)}");

            if (data.ValueKind != JsonValueKind.Array || data.GetArrayLength() == 0)
                return null;

            var assinatura = data[0];
            return new AssinaturaGateway
            {
                Id = Texto(assinatura, "id"),
                Status = Texto(assinatura, "status"),
                ValorCentavos = Inteiro(assinatura, "amount")
            };
        }

        public async Task CancelarAssinatura(string assinaturaId)
        {
            await Enviar(HttpMethod.Post, "subscriptions/cancel", new { id = assinaturaId });
        }

        private async Task<JsonElement> Enviar(HttpMethod metodo, string caminho, object corpo = null)
        {
            using var requisicao = new HttpRequestMessage(metodo, caminho);
            if (corpo != null)
                requisicao.Content = JsonContent.Create(corpo, options: SemNulos);

            using var resposta = await _http.SendAsync(requisicao);
            var conteudo = await resposta.Content.ReadAsStringAsync();

            JsonElement raiz;
            try
            {
                raiz = JsonDocument.Parse(conteudo).RootElement.Clone();
            }
            catch (JsonException)
            {
                _logger.LogError("Resposta inválida do AbacatePay em {Caminho}: {Status}", caminho, (int)resposta.StatusCode);
                throw new PagamentoIndisponivelException("O serviço de pagamento retornou uma resposta inválida.");
            }

            var sucesso = raiz.TryGetProperty("success", out var s) && s.ValueKind == JsonValueKind.True;
            if (!resposta.IsSuccessStatusCode || !sucesso)
            {
                var erro = raiz.TryGetProperty("error", out var e) && e.ValueKind == JsonValueKind.String ? e.GetString() : null;
                _logger.LogError("AbacatePay recusou {Caminho}: {Status} {Erro}", caminho, (int)resposta.StatusCode, erro);
                throw new PagamentoIndisponivelException("Não foi possível falar com o serviço de pagamento.");
            }

            return raiz.TryGetProperty("data", out var data) ? data : default;
        }

        private static CheckoutGateway LerCheckout(JsonElement data) => new CheckoutGateway
        {
            Id = Texto(data, "id"),
            Url = Texto(data, "url"),
            Status = Texto(data, "status"),
            ValorCentavos = Inteiro(data, "amount")
        };

        private static string Texto(JsonElement elemento, string campo)
            => elemento.ValueKind == JsonValueKind.Object
               && elemento.TryGetProperty(campo, out var valor)
               && valor.ValueKind == JsonValueKind.String
                ? valor.GetString()
                : null;

        private static int Inteiro(JsonElement elemento, string campo)
            => elemento.ValueKind == JsonValueKind.Object
               && elemento.TryGetProperty(campo, out var valor)
               && valor.ValueKind == JsonValueKind.Number
                ? valor.GetInt32()
                : 0;
    }
}
