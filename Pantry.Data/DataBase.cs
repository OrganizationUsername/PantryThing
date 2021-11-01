// ReSharper disable UnusedMember.Global
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;

namespace Pantry.Data
{
    //dotnet ef migrations add Initial
    //dotnet ef migrations add foodConstraint
    //dotnet ef database update foodConstraint

    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMember.Global")]
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
        public int FoodInfoId { get; set; }
        // ReSharper disable once IdentifierTypo
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
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<MealInstance> MealInstances { get; set; }
        public DbSet<MealInstanceRow> MealInstanceRows { get; set; }
        public DbSet<MealOfTheDay> MealOfTheDays { get; set; }
        public DbSet<PlannedCookStep> PlannedCookSteps { get; set; }
        public DbSet<ItemReservation> ItemReservations { get; set; }
        public DbSet<PlannedCook> PlannedCooks { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<Unit> Units { get; set; }
        // ReSharper disable once IdentifierTypo
        public DbSet<Inventory> Inventorys { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlite(@"Data Source=..\..\..\..\testDb.db");
        }
    }
}