using AutoMapper;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteCriacaoDto, ClienteModel>();
            CreateMap<ClienteEdicaoDto, ClienteModel>();
        }
    }
}

