using AutoMapper;
using PetShop.API.Dto.Agendamento;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class AgendamentoProfile : Profile
    {
        public AgendamentoProfile()
        {
            CreateMap<AgendamentoModel, AgendamentoDto>()
                .ForMember(dest => dest.NomePet, opt => opt.MapFrom(src => src.Pet.Nome))
                .ForMember(dest => dest.NomeServico, opt => opt.MapFrom(src => src.Servico.Nome))
                .ReverseMap();
        }
    }
}
