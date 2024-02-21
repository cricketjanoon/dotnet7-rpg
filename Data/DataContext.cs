global using Microsoft.EntityFrameworkCore;
using dotnet_rpg.Models;

namespace dotnet_rpg.Data
{
    public class DataContext : DbContext {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Skill>().HasData(
                new Skill(1, "Fireball", 3),
                new Skill(2, "Frenzy",  40),
                new Skill(3, "Blizzard",  50)
            );
        }

        public DbSet<Character> Characters => Set<Character>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Weapon> Weapons => Set<Weapon>();
        public DbSet<Skill> Skills => Set<Skill>();
        
    }
}