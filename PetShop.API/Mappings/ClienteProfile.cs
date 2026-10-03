using AutoMapper;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteDto, ClienteModel>();
            CreateMap<ClienteModel, ClienteDto>();
        }
    }
}
