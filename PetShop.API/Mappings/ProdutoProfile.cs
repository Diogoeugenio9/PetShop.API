using AutoMapper;
using PetShop.API.Dto.Produto;
using PetShop.API.Models;

namespace PetShop.API.Mappings
{
    public class ProdutoProfile : Profile
    {
        public ProdutoProfile()
        {
            CreateMap<ProdutoModeloDto, ProdutoModel>().ReverseMap();
        }
    }
}