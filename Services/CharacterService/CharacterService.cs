using dotnet_rpg.Models;
using dotnet_rpg.DTOs.Character;
using AutoMapper;
using AutoMapper;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }
        private static List<Character> characters = new List<Character>(){
            new Character{Id=1, Name="Sam"},
            new Character{Id=2, Name="Shabs"},
            new Character()        
        };

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.Id = characters.Max(c=>c.Id)+1;
            characters.Add(character);
            serviceResponse.Data = characters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = characters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var character = characters.FirstOrDefault<Character>(c=> c.Id==id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto  updatedCharacter){
            
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try{
                var character = characters.FirstOrDefault(c => c.Id==updatedCharacter.Id);

                if(character==null){
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
                }

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch(Exception ex) {
                serviceResponse.Success=false;
                serviceResponse.Message=ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try{
                // var character = characters.First(c => c.Id==id); First() will throw the exception itself 
                var character = characters.FirstOrDefault(c => c.Id==id);
                if(character is null)
                    throw new Exception($"Character with Id '{id}' not found.");
                
                characters.Remove(character);

                serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch(Exception ex){
                serviceResponse.Success=false;
                serviceResponse.Message=ex.Message;
            }

            return serviceResponse;
        }
    }
}