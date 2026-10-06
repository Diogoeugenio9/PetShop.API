namespace PetShop.API.Models
{
    public class VacinaModel
    {
        public int Id { get; set; }

        public int PetId { get; set; }
        public virtual PetModelo Pet { get; set; }

        public string NomeVacina { get; set; }
        public string Fabricante { get; set; }
        public string Lote { get; set; }
        public DateOnly DataAplicacao { get; set; }
        public DateOnly? DataProximaDose { get; set; }
        public bool DoseUnica { get; set; }
        public string Observacoes { get; set; }
    }
}
