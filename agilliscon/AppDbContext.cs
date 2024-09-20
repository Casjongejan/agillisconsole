using Microsoft.EntityFrameworkCore;
using TesterConsoleApp.People;
using TesterConsoleApp.Pets;

namespace TesterConsoleApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Pet> Pets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./mydatabase.db");
        }
    }
}
