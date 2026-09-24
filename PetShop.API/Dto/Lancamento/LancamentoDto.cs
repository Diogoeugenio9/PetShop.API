namespace PetShop.API.Dto.Lancamento
{
    public class LancamentoDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }
        public DateTime Data { get; set; }
        public string Categoria { get; set; }
        public string FormaPagamento { get; set; }
        public string Observacoes { get; set; }
    }
}