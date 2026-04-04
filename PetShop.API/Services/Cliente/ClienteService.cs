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

       
        public async Task<List<ClienteDto>> ListarClientes()
        {
            var clientes = await _repository.GetAllAsync();
            return _mapper.Map<List<ClienteDto>>(clientes);
        }

        
        public async Task<ClienteDto?> BuscarClientePorId(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            return _mapper.Map<ClienteDto?>(cliente);
        }

        
        public async Task<ClienteDto?> BuscarClientePorIdPet(int idPet)
        {
            var cliente = await _repository.GetByPetId(idPet);
            return _mapper.Map<ClienteDto?>(cliente);
        }

        
        public async Task<ClienteDto> CriarCliente(ClienteDto dto)
        {
            var cliente = _mapper.Map<ClienteModel>(dto);
            await _repository.AddAsync(cliente);
            return _mapper.Map<ClienteDto>(cliente);
        }

          
        public async Task<ClienteDto?> EditarCliente(ClienteDto dto)
        {
            var cliente = await _repository.GetByIdAsync(dto.Id);
            if (cliente == null) return null;

            _mapper.Map(dto, cliente);
            await _repository.UpdateAsync(cliente);

            return _mapper.Map<ClienteDto>(cliente);
        }

        
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
