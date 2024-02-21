using dotnet_rpg.Models;
using dotnet_rpg.DTOs.Character;
using AutoMapper;
using dotnet_rpg.Data;
using System.Security.Claims;

namespace dotnet_rpg.Services.CharacterService {
    public class CharacterService : ICharacterService {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpAccesor) {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpAccesor;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter) {
            
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id==GetUserId());
            
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            var characters = await _context.Characters
                                                .Include(c => c.Skills)
                                                .Include(c => c.Weapon)
                                                .Where(c => c.User!.Id==GetUserId()
                                            ).ToListAsync();
            serviceResponse.Data = characters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters() {
           
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters
                                                .Include(c => c.Skills)
                                                .Include(c => c.Weapon)
                                                .Where(c => c.User!.Id==GetUserId()
                                            ).ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c=> _mapper.Map<GetCharacterDto>(c)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id) {
            
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters
                                                .Include(c => c.Skills)
                                                .Include(c => c.Weapon)
                                                .FirstOrDefaultAsync<Character>(c=> c.Id==id && c.User!.Id==GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter){
            
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try{

                var character = await _context.Characters
                                            .Include(c => c.Skills)
                                            .Include(c => c.Weapon)
                                            .Include(c => c.User)
                                            .FirstOrDefaultAsync(c => c.Id==updatedCharacter.Id);

                if(character == null || character.User!.Id!=GetUserId())
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                // we are not calling seperate update, but modifying the property and calling save changes
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch(Exception ex) {
                serviceResponse.Success=false;
                serviceResponse.Message=ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id) {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try{
                // var character = characters.First(c => c.Id==id); //First() will throw the exception itself 
                var character = _context.Characters.Include(c => c.Weapon).FirstOrDefault(c => (c.Id==id && c.User!.Id==GetUserId()));
                if(character is null)
                    throw new Exception($"Character with Id '{id}' not found.");
                
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters
                                            .Include(c => c.Skills)
                                            .Include(c => c.Weapon)
                                            .Where(c=> c.User!.Id==GetUserId())
                                            .Select(c => _mapper.Map<GetCharacterDto>(c)
                                        ).ToListAsync();
            }
            catch(Exception ex){
                serviceResponse.Success=false;
                serviceResponse.Message=ex.Message;
            }

            return serviceResponse;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto characterSkill)
        {
            var response = new ServiceResponse<GetCharacterDto>();

            try{
                var character = await _context.Characters
                                                    .Include(c => c.Weapon)
                                                    .Include(c => c.Skills)
                                                    .FirstOrDefaultAsync(c => c.Id==characterSkill.CharacterId && c.User!.Id==GetUserId());
                if(character is null)
                    throw new Exception($"No character found with id {characterSkill.CharacterId}");

                var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id==characterSkill.SkillId);
                if(skill is null)
                    throw new Exception($"No skill found with id {characterSkill.SkillId}");

                character.Skills!.Add(skill);

                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

    }
}