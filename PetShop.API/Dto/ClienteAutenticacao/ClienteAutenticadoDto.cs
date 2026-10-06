namespace PetShop.API.Dto.ClienteAutenticacao
{
    public class ClienteAutenticadoDto
    {
        public string Token { get; set; }
        public ClienteResumoDto Cliente { get; set; }
    }

    public class ClienteResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
    }

    public class PetshopPublicoDto
    {
        public int Id { get; set; }
        public string NomeLoja { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}
