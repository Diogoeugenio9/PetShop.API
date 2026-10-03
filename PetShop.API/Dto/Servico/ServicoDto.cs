using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Servico
{
    public class ServicoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do serviço.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string Descricao { get; set; }

        [Range(0, 99999999, ErrorMessage = "Preço inválido.")]
        public decimal Preco { get; set; }

        [Range(0, 1440, ErrorMessage = "Duração deve estar entre 0 e 1440 minutos.")]
        public int DuracaoMinutos { get; set; }

        public bool Ativo { get; set; }
    }
}
