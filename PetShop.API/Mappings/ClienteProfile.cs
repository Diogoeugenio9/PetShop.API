using AutoMapper;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {

            // DTO → Model (necessário para criar/editar)
             CreateMap<ClienteCriacaoDto, ClienteModel>();
             CreateMap<ClienteEdicaoDto, ClienteModel>();

            // Model → DTO (útil para retornar dados)
            CreateMap<ClienteModel, ClienteCriacaoDto>();
            CreateMap<ClienteModel, ClienteEdicaoDto>();
        }
    }
}

