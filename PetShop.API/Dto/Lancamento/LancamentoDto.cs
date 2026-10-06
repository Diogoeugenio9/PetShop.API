using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Lancamento
{
    public class LancamentoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a descrição.")]
        [StringLength(200)]
        public string Descricao { get; set; }

        [Range(0.01, 99999999, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Informe o tipo: receita ou despesa.")]
        public string Tipo { get; set; }

        public DateTime Data { get; set; }

        [StringLength(100)]
        public string Categoria { get; set; }

        [StringLength(50)]
        public string FormaPagamento { get; set; }

        [StringLength(500)]
        public string Observacoes { get; set; }

        public int? AgendamentoId { get; set; }
        public int? ServicoId { get; set; }
    }
}
