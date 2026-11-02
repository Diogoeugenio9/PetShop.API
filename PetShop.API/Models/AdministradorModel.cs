namespace PetShop.API.Models
{
    public class AdministradorModel
    {
        public int Id { get; set; }
        public string NomeProprietario { get; set; }
        public string NomeLoja { get; set; }
        public string Telefone { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; } // nunca salvar senha pura

        public DateTime? CriadoEmUtc { get; set; }
        public DateTime TesteIniciadoEmUtc { get; set; }
        public DateTime TesteTerminaEmUtc { get; set; }
    }
}
