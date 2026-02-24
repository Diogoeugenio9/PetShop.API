using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.API.Models
{
    public class ServicoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        [Column(TypeName = "decimal(10,2)")] 
        public decimal Preco { get; set; }

        public int DuracaoMinutos { get; set; }
        public bool Ativo { get; set; } 
        
        // Relacionamento: 1 Serviço → N Agendamentos ADICIONAR DEPOIS QUE CRIAR AGENDAMENTO
        public virtual ICollection<AgendamentoModel> Agendamentos { get; set; }

    }
}
