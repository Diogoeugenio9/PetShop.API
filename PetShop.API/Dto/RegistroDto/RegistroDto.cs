using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.RegistroDto
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "Informe o nome do proprietário.")]
        [StringLength(150)]
        public string NomeProprietario { get; set; }

        [Required(ErrorMessage = "Informe o nome da loja.")]
        [StringLength(150)]
        public string NomeLoja { get; set; }

        [StringLength(30)]
        public string Telefone { get; set; }

        [StringLength(100)]
        public string Cidade { get; set; }

        [StringLength(50)]
        public string Estado { get; set; }

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; }
    }
}
