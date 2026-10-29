namespace PetShop.API.Models
{
    public class EventoPagamentoModel
    {
        public int Id { get; set; }
        public string EventoId { get; set; }
        public string Evento { get; set; }
        public DateTime RecebidoEm { get; set; }
    }
}
