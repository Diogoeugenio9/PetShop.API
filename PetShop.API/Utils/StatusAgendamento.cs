namespace PetShop.API.Utils
{
    public static class StatusAgendamento
    {
        public const string Pendente = "Pendente";
        public const string Confirmado = "Confirmado";
        public const string Concluido = "Concluido";
        public const string Cancelado = "Cancelado";

        public static readonly string[] Concluiveis = { Pendente, Confirmado, "Agendado" };

        public static bool EhCancelado(string status)
            => !string.IsNullOrWhiteSpace(status)
               && status.Contains("cancel", StringComparison.OrdinalIgnoreCase);

        public static bool EhConcluido(string status)
            => TextoNormalizado.Normalizar(status).StartsWith("conclu");

        public static bool EhReagendado(string status)
            => TextoNormalizado.Normalizar(status).StartsWith("reagend");

        public static bool EstaAberto(string status)
        {
            var normalizado = TextoNormalizado.Normalizar(status);
            return normalizado.Length == 0
                   || Concluiveis.Any(s => TextoNormalizado.Normalizar(s) == normalizado);
        }
    }
}
