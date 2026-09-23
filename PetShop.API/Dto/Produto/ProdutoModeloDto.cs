namespace PetShop.API.Dto.Produto
{
    public class ProdutoModeloDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public int Quantidade { get; set; }
        public string Unidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int EstoqueMinimo { get; set; }
        public string Fornecedor { get; set; }
        public bool Ativo { get; set; }
    }
}