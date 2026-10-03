using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Cliente
{
    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do cliente.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(100)]
        public string Sobrenome { get; set; }

        [StringLength(20)]
        public string Cpf { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Telefone { get; set; }

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
        public string Estado { get; set; }

        public bool Ativo { get; set; }
    }
}
