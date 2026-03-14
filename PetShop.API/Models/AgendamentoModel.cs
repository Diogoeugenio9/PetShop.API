namespace PetShop.API.Models
{
    public class AgendamentoModel
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; }  

        // Relacionamentos
        public int PetId { get; set; }
        public virtual PetModel Pet { get; set; }

        public int ServicoId { get; set; }
        public virtual ServicoModel Servico { get; set; }
    }
}
