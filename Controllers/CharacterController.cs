using Microsoft.AspNetCore.Mvc;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.DTOs.Character;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase {
        private static List<Character> characters = new List<Character>(){
            new Character{Id=1, Name="Sam"},
            new Character{Id=2, Name="Shabs"},
            new Character()        
        };

        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get(){
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet]
        [Route("GetOne/{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingleCharacter(int id){
            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        [Route("AddNewCharacter")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter){
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut]
        [Route("UpdateCharacter")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> UpdateCharacter(UpdateCharacterDto updatedCharacter){
            
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            if(response.Data==null){
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> DeleteCharacter(int id){
            var response = await _characterService.DeleteCharacter(id);
            if(response.Data==null)
                return NotFound(response);
            return Ok(response);
        }

    }
}