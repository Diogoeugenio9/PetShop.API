using PetShop.API.Dto;
using PetShop.API.Dto.LoginDto;
using PetShop.API.Dto.RegistroDto;
using PetShop.API.Models;

namespace PetShop.API.Services
{
    public interface IAuthService
    {
        string Login(LoginDto dto);
        string Registrar(RegistroDto dto);
        string GerarToken(AdministradorModel admin);
    }
}
