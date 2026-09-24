using AutoMapper;
using PetShop.API.Dto.Lancamento;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class LancamentoProfile : Profile
    {
        public LancamentoProfile()
        {
            CreateMap<LancamentoDto, LancamentoModel>().ReverseMap();
        }
    }
}