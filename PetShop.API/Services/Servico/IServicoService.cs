using PetShop.API.Dto.Servico;
using PetShop.API.Models;

namespace PetShop.API.Services.Servico
{
    namespace PetShop.API.Services.Servico
    {
        public interface IServicoService
        {
            
            Task<List<ServicoModel>> ListarServicos();
            Task<ServicoModel?> BuscarServicoPorId(int idServico);

            
            Task<ServicoModel> CriarServico(ServicoDto servicoCriacaoDto);
            Task<ServicoModel?> EditarServico(ServicoDto servicoEdicaoDto);
            Task<bool> ExcluirServico(int idServico);
        }

    }
}

