using AutoMapper;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;

namespace PetShop.API.Services.Pet
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _repository;
        private readonly IMapper _mapper;

        public PetService(IPetRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Listar todos os pets
        public async Task<List<PetModel>> ListarPets()
        {
            return await _repository.GetAll();
        }
        // Buscar pet por ID
        public async Task<PetModel?> BuscarPetPorId(int idPet)
        {
            return await _repository.GetById(idPet);
        }
        // Buscar pets de um cliente
        public async Task<List<PetModel>> BuscarPetPorIdCliente(int idCliente)
        {
            return await _repository.GetByClienteId(idCliente);
        }
        // Criar pet
        public async Task<PetModel> CriarPet(PetCriacaoDto dto)
        {
            var pet = _mapper.Map<PetModel>(dto);
            pet.DataCadastro = DateTime.Now;
            pet.Ativo = true;

            await _repository.Add(pet);
            return pet;
        }

        // Editar pet
        public async Task<PetModel?> EditarPet(PetEdicaoDto dto)
        {
            var pet = await _repository.GetById(dto.Id);

            if (pet == null)
                return null;

            _mapper.Map(dto, pet);
            await _repository.Update(pet);

            return pet;
        }

        // Excluir pet         
        public async Task<bool> ExcluirPet(int idPet)
        {
            var pet = await _repository.GetById(idPet);

            if (pet == null)
                return false;

            await _repository.Delete(pet);
            return true;

        }





    }
}
