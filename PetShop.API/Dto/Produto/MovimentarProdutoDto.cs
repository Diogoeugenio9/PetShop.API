using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Produto
{
    public class MovimentarProdutoDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Informe o produto.")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Informe o tipo: entrada ou saida.")]
        public string Tipo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [StringLength(200)]
        public string Motivo { get; set; }
    }
}
