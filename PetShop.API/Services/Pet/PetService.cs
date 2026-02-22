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

        public async Task<ResponseModel<List<PetModel>>> ListarPets()
        {
            var resposta = new ResponseModel<List<PetModel>>();
            resposta.Dados = await _repository.GetAll();
            resposta.Mensagem = "Pets listados com sucesso!";
            return resposta;
        }

        public async Task<ResponseModel<PetModel>> BuscarPetPorId(int idPet)
        {
            var resposta = new ResponseModel<PetModel>();
            var pet = await _repository.GetById(idPet);

            if (pet == null)
            {
                resposta.Mensagem = "Pet não encontrado!";
                return resposta;
            }

            resposta.Dados = pet;
            resposta.Mensagem = "Pet localizado!";
            return resposta;
        }

        public async Task<ResponseModel<List<PetModel>>> BuscarPetPorIdCliente(int idCliente)
        {
            var resposta = new ResponseModel<List<PetModel>>();
            resposta.Dados = await _repository.GetByClienteId(idCliente);
            resposta.Mensagem = "Pets do cliente listados!";
            return resposta;
        }

        public async Task<ResponseModel<List<PetModel>>> CriarPet(PetCriacaoDto dto)
        {
            var resposta = new ResponseModel<List<PetModel>>();
            var pet = _mapper.Map<PetModel>(dto);

            await _repository.Add(pet);

            resposta.Dados = await _repository.GetAll();
            resposta.Mensagem = "Pet criado com sucesso!";
            return resposta;
        }

        public async Task<ResponseModel<List<PetModel>>> EditarPet(PetEdicaoDto dto)
        {
            var resposta = new ResponseModel<List<PetModel>>();
            var pet = await _repository.GetById(dto.Id);

            if (pet == null)
            {
                resposta.Mensagem = "Pet não encontrado!";
                return resposta;
            }

            _mapper.Map(dto, pet);
            await _repository.Update(pet);

            resposta.Dados = await _repository.GetAll();
            resposta.Mensagem = "Pet atualizado com sucesso!";
            return resposta;
        }

        public async Task<ResponseModel<List<PetModel>>> ExcluirPet(int idPet)
        {
            var resposta = new ResponseModel<List<PetModel>>();
            var pet = await _repository.GetById(idPet);

            if (pet == null)
            {
                resposta.Mensagem = "Pet não encontrado!";
                return resposta;
            }

            await _repository.Delete(pet);

            resposta.Dados = await _repository.GetAll();
            resposta.Mensagem = "Pet removido com sucesso!";
            return resposta;
        }

      

    }
}
