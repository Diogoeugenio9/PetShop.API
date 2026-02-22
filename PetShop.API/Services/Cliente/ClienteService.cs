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

        // Listar todos os clientes
         public async Task<List<ClienteModel>> ListarClientes() 
        {
            return await _repository.GetAll(); 
        }

        //Buscar cleinte por Id
        public async Task<ClienteModel?> BuscarClientePorId(int idCliente)
        {
            return await _repository.GetById(idCliente);
        }


        // Buscar cliente pelo ID do Pet
         public async Task<ClienteModel?> BuscarClientePorIdPet(int idPet)
        {
            return await _repository.GetByPetId(idPet);
        }

        public async Task<ClienteModel> CriarCliente(ClienteCriacaoDto dto)
        {
            var cliente = _mapper.Map<ClienteModel>(dto);
            cliente.DataCadastro = DateTime.Now;
            cliente.Ativo = true;

            await _repository.Add(cliente);
            return cliente;
        }


        // Editar cliente     
         public async Task<ClienteModel?> EditarCliente(ClienteEdicaoDto dto)
        { 
            var cliente = await _repository.GetById(dto.Id);
            
            if (cliente == null)             
                return null;

            _mapper.Map(dto, cliente);
            await _repository.Update(cliente);

            return cliente; 
        }


        // Excluir cliente
        public async Task<bool> ExcluirCliente(int idCliente)
        {
            var cliente = await _repository.GetById(idCliente);

            if (cliente == null) 
                return false;
            
            await _repository.Delete(cliente);
            return true; 


        }
    }
}
