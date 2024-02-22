using dotnet_rpg.DTOs.Figh;
using dotnet_rpg.DTOs.Fight;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.FightService {
    public interface IFightService {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttachDto request);
        Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request);
        Task<ServiceResponse<FightResultDto>> StartFight(FightRequestDto request);
        Task<ServiceResponse<List<HighScoreDto>>> GetHighScore();
    }
}