using AutoMapper;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;
using PetShop.API.Repository.Interface;
using System.Security.Cryptography;

namespace PetShop.API.Services.Pet
{
    public class PetModeloService : IPetModeloService
    {
        private readonly IPetModeloRepository _repository;
        private readonly IMapper _mapper;

        public PetModeloService(IPetModeloRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PetModelo> BuscarPetPorId(int idPet)
        {
            return await _repository.GetByIdAsync(idPet);
        }

        public async Task<List<PetModelo>> BuscarPetPorIdCliente(int idCliente)
        {
            return await _repository.GetByClienteIdAsync(idCliente);
        }

        public async Task<PetModelo> CriarPet(PetModeloDto petCriacaoDto)
        {
            var pet = _mapper.Map<PetModelo>(petCriacaoDto);
            
            pet.DataCadastro = DateTime.Now;
            pet.Ativo = true;

            await _repository.AddAsync(pet);
            return pet;
        }

        public async Task<PetModelo> EditarPet(PetModeloDto petEdicaoDto)
        {
            var pet = await _repository.GetByIdAsync(petEdicaoDto.Id);

            if (pet == null)
                return null;

            _mapper.Map(petEdicaoDto, pet);
            await _repository.UpdateAsync(pet);

            return pet;
        }

        public async Task<bool> ExcluirPet(int idPet)
        {
            var pet = await _repository.GetByIdAsync(idPet);

            if (pet == null)
                return false;

            await _repository.DeleteAsync(pet);
            return true;
        }

        public async Task<List<PetModelo>> ListarPets()
        {
            return await _repository.GetAllAsync();
        }
    }
}