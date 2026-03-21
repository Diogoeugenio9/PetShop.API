namespace PetShop.API.Models
{
    public class PetModelo
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public string Especie { get; set; }
        public string Raca { get; set; }
        public int Idade { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }


        public int ClienteId { get; set; }

        // RELACIONAMENTO DE BANCO DE DADOS
        public virtual ClienteModel Cliente { get; set; }

    }
}