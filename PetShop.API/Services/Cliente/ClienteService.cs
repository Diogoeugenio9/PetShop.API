using AutoMapper;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;
using PetShop.API.Repositories.Cliente;
using static PetShop.API.Repository.Interface.IClienteRepository;

namespace PetShop.API.Services.Cliente
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseModel<List<ClienteModel>>> ListarClientes()
        {
            var resposta = new ResponseModel<List<ClienteModel>>();
            resposta.Dados = await _repository.GetAll();
            resposta.Mensagem = "Clientes listados com sucesso!";
            return resposta;
        }

        public async Task<ResponseModel<ClienteModel>> BuscarClientePorId(int idCliente)
        {
            var resposta = new ResponseModel<ClienteModel>();
            var cliente = await _repository.GetById(idCliente);

            if (cliente == null)
            {
                resposta.Mensagem = "Cliente não encontrado!";
                return resposta;
            }

            resposta.Dados = cliente;
            resposta.Mensagem = "Cliente localizado!";
            return resposta;
        }

        public async Task<ResponseModel<ClienteModel>> BuscarClientePorIdPet(int idPet)
        {
            var resposta = new ResponseModel<ClienteModel>();
            var cliente = await _repository.GetByPetId(idPet);

            if (cliente == null)
            {
                resposta.Mensagem = "Cliente não encontrado!";
                return resposta;
            }

            resposta.Dados = cliente;
            resposta.Mensagem = "Cliente localizado!";
            return resposta;
        }

        public async Task<ResponseModel<List<ClienteCriacaoDto>>> CriarCliente(ClienteCriacaoDto dto)
        {
            var resposta = new ResponseModel<List<ClienteCriacaoDto>>();
            var cliente = _mapper.Map<ClienteModel>(dto);

            cliente.DataCadastro = DateTime.Now;
            cliente.Ativo = true;

            await _repository.Add(cliente);

            resposta.Mensagem = "Cliente criado com sucesso!";
            return resposta;
        }

        public async Task<ResponseModel<List<ClienteEdicaoDto>>> EditarCliente(ClienteEdicaoDto dto)
        {
            var resposta = new ResponseModel<List<ClienteEdicaoDto>>();
            var cliente = await _repository.GetById(dto.Id);

            if (cliente == null)
            {
                resposta.Mensagem = "Cliente não encontrado!";
                return resposta;
            }

            _mapper.Map(dto, cliente);
            await _repository.Update(cliente);

            resposta.Mensagem = "Cliente atualizado com sucesso!";
            return resposta;
        }

        public async Task<ResponseModel<List<ClienteModel>>> ExcluirCliente(int idCliente)
        {
            var resposta = new ResponseModel<List<ClienteModel>>();
            var cliente = await _repository.GetById(idCliente);

            if (cliente == null)
            {
                resposta.Mensagem = "Cliente não encontrado!";
                return resposta;
            }

            await _repository.Delete(cliente);
            resposta.Mensagem = "Cliente removido com sucesso!";
            return resposta;
        }
    }
}
