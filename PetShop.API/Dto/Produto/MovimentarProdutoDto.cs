namespace PetShop.API.Dto.Produto
{
    public class MovimentarProdutoDto
    {
        public int ProdutoId { get; set; }
        public string Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Motivo { get; set; }
    }
}