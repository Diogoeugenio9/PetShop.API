using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Portal
{
    public class PerfilClienteDto
    {
        public int Id { get; set; }

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

        public EnderecoDto Endereco { get; set; }
    }

    public class EnderecoDto
    {
        [StringLength(10)]
        public string Cep { get; set; }

        [StringLength(150)]
        public string Logradouro { get; set; }

        [StringLength(20)]
        public string Numero { get; set; }

        [StringLength(100)]
        public string Bairro { get; set; }

        [StringLength(100)]
        public string Cidade { get; set; }

        [StringLength(50)]
        public string Uf { get; set; }
    }
}
