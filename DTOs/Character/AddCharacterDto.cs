using dotnet_rpg.Models;

namespace dotnet_rpg.DTOs.Character
{
    public class AddCharacterDto
    {
        public int Id {  get; set; }
        public string Name { get; set; } = "Frodo";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public int Intelligence { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.Knight;        
    }
}