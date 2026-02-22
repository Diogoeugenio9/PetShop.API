namespace PetShop.API.Dto.Pet
{
    public class PetCriacaoDto
    {
        public string Nome { get; set; }
        public int ClienteId { get; set; }

        public string Especie { get; set; }
        public string Raca { get; set; }
        public int Idade { get; set; }
        public bool Ativo { get; set; }
    }
}
