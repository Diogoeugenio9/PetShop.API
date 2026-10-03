using AutoMapper;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class PetModeloProfile : Profile
    {
        public PetModeloProfile()
        {
            CreateMap<PetModeloDto, PetModelo>()
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
                .ForMember(dest => dest.Cliente, opt => opt.Ignore());

            CreateMap<PetModelo, PetModeloDto>();
        }
    }
}
