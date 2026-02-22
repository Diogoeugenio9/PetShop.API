using AutoMapper;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class PetProfile : Profile
    {
        public PetProfile()
        {
            CreateMap<PetCriacaoDto, PetModel>();
            CreateMap<PetEdicaoDto, PetModel>();
        }
    }
}
