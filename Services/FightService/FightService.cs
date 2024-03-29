using AutoMapper;
using dotnet_rpg.DTOs.Figh;
using dotnet_rpg.DTOs.Fight;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.FightService {
    public class FightService : IFightService {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public FightService(DataContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request) {
            
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters.Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _context.Characters.Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Skills is null)
                    throw new Exception("One or more of the following {Attacker, Oponent, Attacker Weapon} is null.");

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill is null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know his skill.";
                    return response;
                }

                int damage = DoSkillDamage(attacker, opponent, skill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated.";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OppenentHP = opponent.HitPoints,
                    Damage = damage
                };

            }
            catch (Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoSkillDamage(Character attacker, Character opponent, Skill skill) {
            
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defeats);

            if (damage > 0)
                opponent.HitPoints -= damage;

            return damage;
        }

        public async Task<ServiceResponse<FightResultDto>> StartFight(FightRequestDto request) {
            var response = new ServiceResponse<FightResultDto> {Data = new FightResultDto()};

            try{
                var characters = await _context.Characters.Include(c => c.Weapon).Include(c => c.Skills).Where(c => request.CharasterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;

                while(!defeated){

                    foreach (var attacker in characters){

                        var opponents = characters.Where(c => c.Id!=attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if(useWeapon && attacker.Weapon!=null){
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent); 
                        }
                        else if (!useWeapon && attacker.Skills!=null){
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillDamage(attacker, opponent, skill);
                        }
                        else{
                            response.Data.Log.Add($"{attacker.Name} wasn't able to attack!");
                            continue;
                        }
                        
                        response.Data.Log.Add($"{attacker.Name} attacked {opponent.Name} using {attackUsed} with {(damage >= 0  ? damage : 0)}.");
                        
                        if(opponent.HitPoints <= 0){
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated.");
                            response.Data.Log.Add($"{attacker.Name} winds with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c => {c.Fights++; c.HitPoints=100;});
                await _context.SaveChangesAsync();
            }
            catch(Exception ex){
                response.Message = ex.Message;
                response.Success = false;
            }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttachDto request) {
            
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Weapon is null)
                    throw new Exception("One or more of the following {Attacker, Oponent, Attacker Weapon} is null.");

                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated.";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OppenentHP = opponent.HitPoints,
                    Damage = damage
                };

            }
            catch (Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private int DoWeaponAttack(Character attacker, Character opponent) {
            
            if(attacker.Weapon is null)
                throw new Exception("Attacker has no weapon.");

            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defeats);

            if (damage > 0)
                opponent.HitPoints -= damage;
            
            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore() {
            
            var characters = await _context.Characters.Where(c => c.Fights>0).OrderByDescending(c => c.Victories).ThenBy(c => c.Defeats).ToListAsync();

            var response = new ServiceResponse<List<HighScoreDto>>() {
                Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList()
            };

            return response;
        }
    }
}