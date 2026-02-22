namespace PetShop.API.Models
{
    public class Login
    {
        public int Id { get; set; }

        // Identificação
        public string Nome { get; set; }
        public string Email { get; set; }   // Login principal (único)

        // Segurança
        public string SenhaHash { get; set; }
        public string SenhaSalt { get; set; }

        // Controle
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }

        // Perfil / autorização
        public string Perfil { get; set; } // Admin, Atendente, Veterinário
    }
}
