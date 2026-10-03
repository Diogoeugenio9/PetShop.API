namespace PetShop.API.Utils
{
    public static class StatusAgendamento
    {
        public static bool EhCancelado(string status)
            => !string.IsNullOrWhiteSpace(status)
               && status.Contains("cancel", StringComparison.OrdinalIgnoreCase);
    }
}
