using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;

namespace Pantry.Data
{
    //dotnet-ef migrations add Initial
    public class DataBase : DbContext
    {
        public DbSet<BetterRecipe> BetterRecipes { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodInstance> FoodInstances { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>().Ignore(x => x.BookedTimes);
            modelBuilder.Entity<BetterRecipe>().HasKey(x => x.RecipeId);
            //base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            Assembly asm = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(asm.Location);
            string fullPath = System.IO.Path.Combine(path, "Pantry.db");
            optionsBuilder.UseSqlite(@$"Filename={fullPath}");
        }
    }
}