namespace dotnet_rpg.Models {
    public class Skill {
        public Skill(int id, string name, int damage) {
            Id=id;
            Name=name;
            Damage=damage;
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public List<Character>? Characters { get; set; }
    }
}