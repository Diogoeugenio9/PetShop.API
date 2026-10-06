using System.Security.Claims;

namespace PetShop.API.Utils
{
    public static class UsuarioClaims
    {
        public const string ClienteId = "clienteId";
        public const string PetshopId = "petshopId";

        public static bool EhCliente(this ClaimsPrincipal usuario)
            => usuario?.FindFirst(ClienteId) != null;

        public static int ObterClienteId(this ClaimsPrincipal usuario)
            => LerInteiro(usuario, ClienteId);

        public static int ObterPetshopId(this ClaimsPrincipal usuario)
            => LerInteiro(usuario, PetshopId);

        public static int ObterAdministradorId(this ClaimsPrincipal usuario)
        {
            if (usuario?.Identity?.IsAuthenticated != true)
                return 0;

            if (usuario.EhCliente())
                return usuario.ObterPetshopId();

            var valor = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? usuario.FindFirst("nameid")?.Value;

            return int.TryParse(valor, out var id) ? id : 0;
        }

        private static int LerInteiro(ClaimsPrincipal usuario, string tipo)
        {
            var valor = usuario?.FindFirst(tipo)?.Value;
            return int.TryParse(valor, out var id) ? id : 0;
        }
    }
}
