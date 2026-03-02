using AutoMapper;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

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
        public async Task<List<ClienteDto>> ListarClientes()
        {
            var clientes = await _repository.GetAllAsync();
            return _mapper.Map<List<ClienteDto>>(clientes);
        }

        // Buscar cliente por Id
        public async Task<ClienteDto?> BuscarClientePorId(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            return _mapper.Map<ClienteDto?>(cliente);
        }

        // Buscar cliente pelo ID do Pet
        public async Task<ClienteDto?> BuscarClientePorIdPet(int idPet)
        {
            var cliente = await _repository.GetByPetId(idPet);
            return _mapper.Map<ClienteDto?>(cliente);
        }

        // Criar cliente
        public async Task<ClienteDto> CriarCliente(ClienteDto dto)
        {
            var cliente = _mapper.Map<ClienteModel>(dto);
            await _repository.AddAsync(cliente);
            return _mapper.Map<ClienteDto>(cliente);
        }

        // Editar cliente     
        public async Task<ClienteDto?> EditarCliente(ClienteDto dto)
        {
            var cliente = await _repository.GetByIdAsync(dto.Id);
            if (cliente == null) return null;

            _mapper.Map(dto, cliente);
            await _repository.UpdateAsync(cliente);

            return _mapper.Map<ClienteDto>(cliente);
        }

        // Excluir cliente
        public async Task<bool> ExcluirCliente(int idCliente)
        {
            var cliente = await _repository.GetByIdAsync(idCliente);

            if (cliente == null)
                return false;

            await _repository.Delete(cliente);
            return true;
        }
    }
}
