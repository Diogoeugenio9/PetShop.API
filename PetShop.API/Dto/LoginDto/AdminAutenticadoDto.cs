namespace PetShop.API.Dto.LoginDto
{
    public class AdminAutenticadoDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public int PetshopId { get; set; }
    }
}
