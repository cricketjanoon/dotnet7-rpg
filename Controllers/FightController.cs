using dotnet_rpg.DTOs.Figh;
using dotnet_rpg.DTOs.Fight;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers{

    [ApiController]
    [Route("[controller]")]
    public class FightController : ControllerBase {

        private readonly IFightService _fightService;
        public FightController(IFightService fightService) {
            _fightService = fightService;
        }

        [HttpPost("AttachWithWeapon")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttach(WeaponAttachDto request){
            return Ok(await _fightService.WeaponAttack(request));
        }

        [HttpPost("AttackWithSkill")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> AttackWithSkill(SkillAttackDto request){
            return Ok(await _fightService.SkillAttack(request));
        }

        [HttpPost("StartFight")]
        public async Task<ActionResult<ServiceResponse<FightResultDto>>> StartFight(FightRequestDto request){
            return Ok(await _fightService.StartFight(request));
        }

        [HttpGet("GetHighScore")]
        public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighScore(){
            return Ok(await _fightService.GetHighScore());
        }
    }
}