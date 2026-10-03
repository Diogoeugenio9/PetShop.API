using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Produto
{
    public class ProdutoModeloDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do produto.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(100)]
        public string Categoria { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa.")]
        public int Quantidade { get; set; }

        [StringLength(20)]
        public string Unidade { get; set; }

        [Range(0, 99999999, ErrorMessage = "Preço inválido.")]
        public decimal PrecoUnitario { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Estoque mínimo não pode ser negativo.")]
        public int EstoqueMinimo { get; set; }

        [StringLength(150)]
        public string Fornecedor { get; set; }

        public bool Ativo { get; set; }
    }
}
