namespace PetShop.API.Utils
{
    public static class PlanoAssinatura
    {
        public const string Mensal = "mensal";
        public const string Vitalicio = "vitalicio";

        public const decimal ValorMensal = 40.00m;
        public const decimal ValorVitalicio = 1000.00m;

        public static bool EhValido(string plano) => plano == Mensal || plano == Vitalicio;

        public static decimal ValorDe(string plano) => plano == Vitalicio ? ValorVitalicio : ValorMensal;
    }
}
