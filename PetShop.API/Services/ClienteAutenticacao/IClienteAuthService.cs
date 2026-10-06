using PetShop.API.Dto.ClienteAutenticacao;

namespace PetShop.API.Services.ClienteAutenticacao
{
    public interface IClienteAuthService
    {
        Task<ClienteAutenticadoDto> Login(ClienteLoginDto dto);

        Task<ClienteAutenticadoDto> Registrar(ClienteRegistroDto dto);

        Task<List<PetshopPublicoDto>> ListarPetshops();
    }
}
