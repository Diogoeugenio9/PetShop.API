using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PetShop.API.Services.Assinatura;

namespace PetShop.API.Utils
{
    public class AcessoPetshopFilter : IAsyncActionFilter
    {
        public const string CodigoExpirado = "ACESSO_PETSHOP_EXPIRADO";

        private readonly IAssinaturaService _assinaturaService;

        public AcessoPetshopFilter(IAssinaturaService assinaturaService)
        {
            _assinaturaService = assinaturaService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var usuario = context.HttpContext.User;
            var liberado = context.ActionDescriptor.EndpointMetadata.OfType<LiberadoSemAssinaturaAttribute>().Any();

            if (liberado || usuario?.Identity?.IsAuthenticated != true || usuario.EhCliente())
            {
                await next();
                return;
            }

            var administradorId = usuario.ObterAdministradorId();
            if (administradorId > 0 && !await _assinaturaService.AcessoPermitido(administradorId))
            {
                const string mensagem = "O período de teste terminou e não há assinatura ativa.";
                context.Result = new ObjectResult(new { codigo = CodigoExpirado, message = mensagem, mensagem })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            await next();
        }
    }
}
