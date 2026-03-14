using Microsoft.AspNetCore.Mvc;
using PetShop.API.Dto.Pet;
using PetShop.API.Models;
using PetShop.API.Services.Pet;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetModeloService _petService;

        public PetController(IPetModeloService petService)
        {
            _petService = petService;
        }

        
        [HttpGet("ListarPets")]
        public async Task<ActionResult<List<PetModel>>> ListarPets()
        {
            var pets = await _petService.ListarPets();
            return Ok(pets);
        }

        
        [HttpGet("BuscarPetPorId/{idPet}")]
        public async Task<ActionResult<PetModel>> BuscarPetPorId(int idPet)
        {
            var pet = await _petService.BuscarPetPorId(idPet);
            if (pet == null)
                return NotFound("Pet não encontrado");

            return Ok(pet);
        }

        
        [HttpGet("BuscarPetPorIdCliente/{idCliente}")]
        public async Task<ActionResult<List<PetModel>>> BuscarPetPorIdCliente(int idCliente)
        {
            var pets = await _petService.BuscarPetPorIdCliente(idCliente);
            if (pets == null || !pets.Any())
                return NotFound("Nenhum pet encontrado para este cliente");

            return Ok(pets);
        }

       
        [HttpPost("CriarPet")]
        public async Task<ActionResult<PetModel>> CriarPet(PetModeloDto petCriacaoDto)
        {
            var pet = await _petService.CriarPet(petCriacaoDto);
            return CreatedAtAction(nameof(BuscarPetPorId), new { idPet = pet.Id }, pet);
        }

        
        [HttpPut("EditarPet")]
        public async Task<ActionResult<PetModel>> EditarPet(PetModeloDto petEdicaoDto)
        {
            var pet = await _petService.EditarPet(petEdicaoDto);
            if (pet == null)
                return NotFound("Pet não encontrado");

            return Ok(pet);
        }

        
        [HttpDelete("ExcluirPet/{idPet}")]
        public async Task<IActionResult> ExcluirPet(int idPet)
        {
            var sucesso = await _petService.ExcluirPet(idPet);
            if (!sucesso)
                return NotFound("Pet não encontrado");

            return NoContent();
        }
    }
}
