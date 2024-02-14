global using Microsoft.EntityFrameworkCore;
using dotnet_rpg.Models;

namespace dotnet_rpg.Data
{
    public class DataContext : DbContext {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
            
        }

        public DbSet<Character> Characters => Set<Character>();
    }
}