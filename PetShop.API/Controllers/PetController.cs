using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Cliente;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;
using PetShop.API.Services.Cliente;
using PetShop.API.Services.Pet;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {

        private readonly IPetService _petInterface;
        public PetController(IPetService petInterface)
        {
            _petInterface = petInterface;
        }

        [HttpGet("ListarPets")]
        public async Task<ActionResult<ResponseModel<List<PetModel>>>> ListarPets()
        {
            var pets = await _petInterface.ListarPets();
            return Ok(pets);
        }

        [HttpGet("BuscarPetPorId/{idPet}")]
        public async Task<ActionResult<ResponseModel<PetModel>>> BuscarPetPorId(int idPet)
        {
            var pet = await _petInterface.BuscarPetPorId(idPet);
            return Ok(pet);
        }

        [HttpGet("BuscarPetPorIdCliente/{idCliente}")]
        public async Task<ActionResult<ResponseModel<ClienteModel>>> BuscarPetPorIdCliente(int idCliente)
        {
            var pet = await _petInterface.BuscarPetPorIdCliente(idCliente);
            return Ok(pet);
        }

        [HttpPost("CriarPet")]
        public async Task<ActionResult<ResponseModel<List<PetModel>>>> CriarPet(PetCriacaoDto petCriacaoDto)
        {
            var pets = await _petInterface.CriarPet(petCriacaoDto);
            return Ok(pets);
        }

        [HttpPut("EditarPet")]
        public async Task<ActionResult<ResponseModel<List<PetModel>>>> EditarPet(PetEdicaoDto petEdicaoDto)
        {
            var pets = await _petInterface.EditarPet(petEdicaoDto);
            return Ok(pets);
        }

        [HttpDelete("ExcluirPet")]
        public async Task<ActionResult<ResponseModel<List<PetModel>>>> ExcluirPet(int idPet)
        {
            var pets = await _petInterface.ExcluirPet(idPet);
            return Ok(pets);
        }
    }




}
