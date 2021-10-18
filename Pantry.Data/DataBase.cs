using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;

namespace Pantry.Data
{
    //dotnet ef migrations add Initial
    //dotnet ef migrations add foodConstraint
    //dotnet ef database update foodConstraint

    public enum TemperatureState
    {
        Unopened = 1,
        RoomTemperature = 2,
        Refrigerated = 4,
        Frozen = 6,
    }

    public class ExpiryInformation
    {
        public FoodInfo FoodInfo { get; set; }
        public int FoodInfoId { get; set; }
        public TemperatureState TemperatureState { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }

    public class Manufacturer
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
    }

    public class FoodInfo
    {
        private IEnumerable<string> asdf;
        public int FoodInfoId { get; set; }
        public ICollection<ExpiryInformation> ExpiryInformations { get; set; }
        public string UnitName { get; set; }
        public double UnitConversion { get; set; }
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
        public DbSet<LocationFoods> LocationFoods { get; set; }
        public DbSet<FoodTag> FoodTags { get; set; }
        public DbSet<FoodFoodTag> FoodFoodTags { get; set; }
        public DbSet<RecipeTag> RecipeTags { get; set; }
        public DbSet<RecipeRecipeTag> RecipeRecipeTags { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Inventory> Inventorys { get; set; }

        public DataBase()
        {
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            //optionsBuilder.UseSqlite(@"Data Source=..\..\..\..\testDb.db");
            //C:\Programming\TotallyNewThing\Pantry
            optionsBuilder.UseSqlite(@"Data Source=C:\Programming\TotallyNewThing\Pantry\testDb.db");
        }
    }
}