namespace PetShop.API.Dto.Servico
{
    public class ServicoDto
    {
        public int Id { get; set; } 
        public string Nome { get; set; } 
        public string Descricao { get; set; } 
        public decimal Preco { get; set; } 
        public int DuracaoMinutos { get; set; } 
        public bool Ativo { get; set; }
    }
}
