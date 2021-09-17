using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;

namespace Pantry.Data
{
    //dotnet ef migrations add Initial
    //dotnet ef migrations add foodConstraint
    //dotnet ef database update foodConstraint

    public class DataBase : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<RecipeFood> RecipeFoods { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentCommitment> EquipmentCommitments { get; set; }
        public DbSet<Location> Locations { get; set; }

        //Wow... This should all be refactored to reflect positive numbers for
        //inputs and negative numbers for outputs.



        //public DbSet<Equipment> Equipments { get; set; }
        //public DbSet<BetterRecipeInput> BetterRecipeInputs { get; set; }

        public DataBase()
        {
            //this.Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Recipe>().Ignore(x => x.RecipeSteps);
            modelBuilder.Entity<RecipeStep>()
                .Ignore(x => x.Equipments)
                .HasIndex(x => x.Order);
            //modelBuilder.Entity<Recipe>().HasMany(x => x.RecipeFoods);
            //modelBuilder.Entity<Recipe>().HasMany(x => x.Outputs);
            //modelBuilder.Entity<Recipe>().HasKey(x => x.RecipeFoodId);
            //modelBuilder.Entity<Food>().HasIndex(x => x.FoodName).IsUnique();
            //modelBuilder.Entity<BetterRecipeInput>().HasKey(x => x.BetterRecipeInputId);
            base.OnModelCreating(modelBuilder);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            Assembly asm = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(asm.Location);
            string fullPath = System.IO.Path.Combine(path, "Pantry1.db");
            Console.WriteLine(fullPath);
            optionsBuilder.UseSqlite(@$"Data Source={fullPath}");
        }
    }
}