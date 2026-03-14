using AutoMapper;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;

public class PetModeloProfile : Profile 
{
    public PetModeloProfile()
    {
        CreateMap<PetModeloDto, PetModel>();
        CreateMap<PetModel, PetModeloDto>(); 
    }
}