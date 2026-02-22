using System.Text.Json.Serialization;

namespace PetShop.API.Models
{
    public class ClienteModel
    {
        public int Id { get; set; }

        //Dados pessoais
        public string Nome { get; set; }
        public string Sobrenome { get; set; }

        public string Cpf { get; set; } // Criado agr

        //Contato
        public string Email { get; set; }
        public string Telefone { get; set; }

        // Endereço
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        // Controle
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }



        [JsonIgnore]
        public ICollection<PetModel> Pets { get; set; }
    }
}
