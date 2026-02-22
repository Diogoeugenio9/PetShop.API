using AutoMapper;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class PetProfile : Profile
    {
        public PetProfile()
        {
            // DTO → Model (necessário para criar/editar)
            CreateMap<PetCriacaoDto, PetModel>();
            CreateMap<PetEdicaoDto, PetModel>();

            // Model → DTO (útil para retornar dados)
            CreateMap<PetModel, PetCriacaoDto>();
            CreateMap<PetModel, PetEdicaoDto>();
        }
    }
}
