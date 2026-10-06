using AutoMapper;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteDto, ClienteModel>()
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
                .ForMember(dest => dest.AdministradorId, opt => opt.Ignore())
                .ForMember(dest => dest.Excluido, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
                .ForMember(dest => dest.Pets, opt => opt.Ignore());
            CreateMap<ClienteModel, ClienteDto>();
        }
    }
}
