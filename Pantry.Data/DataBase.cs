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

    public interface IDataBase
    {
        DbSet<Recipe> Recipes { get; set; }
        DbSet<Food> Foods { get; set; }
        DbSet<RecipeFood> RecipeFoods { get; set; }
        DbSet<RecipeStep> RecipeSteps { get; set; }
        DbSet<Equipment> Equipments { get; set; }
        DbSet<EquipmentCommitment> EquipmentCommitments { get; set; }
        DbSet<Location> Locations { get; set; }
        int SaveChanges();
    }

    public class DataBase : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<RecipeFood> RecipeFoods { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentCommitment> EquipmentCommitments { get; set; }
        public DbSet<Location> Locations { get; set; }

        /*
        Another simple recipe is just putting stuff in plastic storage containers.
        I also need a `Containers`
        Need a table of food instances=> Inventory.
            Time Created, Amount, stackable, recipe used, Expiration Date
            I could stack items if they were created at around the same time and they're stackable
        How do I deal with items that might have a few minutes of downtime but must be checked by a human once in a while? Prioritize it? (bbq / cakes)
        What if there were a `FoodInfo`
            Allowable storage time at different temperatures, Nutrition
        have `InventoryAdjustment`
            InventoryAdjustmentReasons, 
        InventoryAdjustmentReasons
            Spoiled, Lost, Found, Purchased
        `Products`
         */

        public DataBase()
        {
            this.Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Recipe>().Ignore(x => x.RecipeSteps);
            modelBuilder.Entity<RecipeStep>()
                //.Ignore(x => x.Equipments)
                .HasIndex(x => x.Order); //pre-optimization
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
            optionsBuilder.UseSqlite(@"Data Source=C:\Programming\TotallyNewThing\Pantry\testDb.db");
        }
    }
}