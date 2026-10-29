using System.ComponentModel.DataAnnotations;

namespace PetShop.API.Dto.Assinatura
{
    public class CheckoutAssinaturaDto
    {
        [Required(ErrorMessage = "Informe o plano.")]
        public string Plano { get; set; }
    }
}
