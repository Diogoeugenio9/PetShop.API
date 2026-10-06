using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.ClienteAutenticacao
{
    public class ClienteRegistroDto
    {
        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(100)]
        public string Sobrenome { get; set; }

        [StringLength(20)]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Informe a senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; }

        public int? PetshopId { get; set; }
    }
}
