using PetShop.API.Dto.LoginDto;
using PetShop.API.Dto.RegistroDto;
using PetShop.API.Models;

namespace PetShop.API.Services
{
    public interface IAuthService
    {
        AdminAutenticadoDto Login(LoginDto dto);
        AdminAutenticadoDto Registrar(RegistroDto dto);
        string GerarToken(AdministradorModel admin);
    }
}
