using System.Globalization;

namespace PetShop.API.Utils
{
    public static class DataHoraBrasil
    {
        private static readonly TimeZoneInfo Fuso = ObterFuso();

        public static readonly CultureInfo Cultura = new CultureInfo("pt-BR");

        public static DateTime Agora => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Fuso);

        public static DateTime Hoje => Agora.Date;

        private static TimeZoneInfo ObterFuso()
        {
            foreach (var id in new[] { "America/Sao_Paulo", "E. South America Standard Time" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }

            return TimeZoneInfo.CreateCustomTimeZone("Brasilia", TimeSpan.FromHours(-3), "Brasília", "Brasília");
        }
    }
}
