using System.Globalization;
using System.Text;

namespace PetShop.API.Utils
{
    public static class TextoNormalizado
    {
        public static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var decomposto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var resultado = new StringBuilder(decomposto.Length);

            foreach (var c in decomposto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    resultado.Append(c);
            }

            return resultado.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
