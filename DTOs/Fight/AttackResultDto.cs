namespace dotnet_rpg.DTOs.Fight {
    public class AttackResultDto {
        public string Attacker { get; set; } = string.Empty;
        public string Opponent { get; set; } = string.Empty;
        public int AttackerHP { get; set; } 
        public int OppenentHP { get; set; }
        public int Damage { get; set; }
    }
}