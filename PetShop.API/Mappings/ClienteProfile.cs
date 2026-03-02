using AutoMapper;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<ClienteDto, ClienteModel>();
        CreateMap<ClienteModel, ClienteDto>();
    }
}
