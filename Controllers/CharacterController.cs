using Microsoft.AspNetCore.Mvc;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.DTOs.Character;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace dotnet_rpg.Controllers {

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase {

        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService) {

            _characterService = characterService;
        }

        [HttpGet]
        [Route("GetOne/{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingleCharacter(int id){

            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAllCharacter(){

            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _characterService.GetAllCharacters(userId));
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