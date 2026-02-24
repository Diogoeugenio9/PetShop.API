using AutoMapper;
using PetShop.API.Dto.Pet;
using PetShop.API.Dto.Servico;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class ServicoProfile : Profile
    {
        public ServicoProfile()
        {
            CreateMap<ServicoDto, ServicoModel>().ReverseMap();
        }


    }
}
