﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Food;
using PantryWPF.Inventory;
using PantryWPF.Recipes;
using Unity;

namespace PantryWPF.Main
{
    public class MainWindowViewModel : VmBase
    {
        public VmBase MainView { get; set; }
        public string VmName { get; set; }
        public NavigationCommand RecipeNavigationCommand { get; set; }
        public NavigationCommand InventoryNavigationCommand { get; set; }
        public NavigationCommand FoodNavigationCommand { get; set; }
        public DelegateCommand SeedDatabaseCommand { get; set; }

        public MainWindowViewModel()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<DataBase, DataBase>();

            MainView = new RecipesListViewModel();
            RecipeNavigationCommand = new NavigationCommand(this, new RecipesListViewModel());
            InventoryNavigationCommand = new NavigationCommand(this, new InventoryViewModel());
            FoodNavigationCommand = new NavigationCommand(this, new FoodListViewModel());
            SeedDatabaseCommand = new DelegateCommand(SeedDatabase);
        }

        public void SeedDatabase()
        {
            PopulateFoods();
            PopulateLocations();
            PopulateEquipment();
            PopulateRecipes();
            PopulateItems();
            PopulateInventory();
        }

        public void PopulateItems()
        {
            using (var dbContext = new DataBase())
            {
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM Items;");
                dbContext.Items.Add(new Item() { Food = dbContext.Foods.First(x => x.FoodName == "Frozen Chicken"), Weight = 4540, Upc = "100Chicken" });
                dbContext.Items.Add(new Item() { Food = dbContext.Foods.First(x => x.FoodName == "BBQ Sauce"), Weight = 454, Upc = "200BBQ" });
                dbContext.Items.Add(new Item() { Food = dbContext.Foods.First(x => x.FoodName == "Milk"), Weight = 3900, Upc = "1GalMilk" });
                _ = dbContext.SaveChanges();
            }
        }

        public void PopulateInventory()
        {
            using (var dbContext = new DataBase())
            {
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM LocationFoods;");

                var defaultLocation = dbContext.Locations.First(x => x.LocationName == "Default");
                var now = DateTime.Now;

                _ = dbContext.LocationFoods.Add(new LocationFoods()
                {
                    Item = dbContext.Items.First(x => x.Upc == "1GalMilk"),
                    ExpiryDate = now + TimeSpan.FromDays(14),
                    Quantity = 3900,
                    Location = defaultLocation,
                });

                _ = dbContext.LocationFoods.Add(new LocationFoods()
                {
                    Item = dbContext.Items.First(x => x.Upc == "1GalMilk"),
                    OpenDate = now + TimeSpan.FromDays(-7),
                    ExpiryDate = now + TimeSpan.FromDays(1),
                    Quantity = 3900,
                    Location = defaultLocation,
                });

                _ = dbContext.LocationFoods.Add(new LocationFoods()
                {
                    Item = dbContext.Items.First(x => x.Upc == "100Chicken"),
                    ExpiryDate = now + TimeSpan.FromDays(180),
                    Quantity = 4540 / 2.0,
                    Location = defaultLocation,
                });

                _ = dbContext.LocationFoods.Add(new LocationFoods()
                {
                    Item = dbContext.Items.First(x => x.Upc == "100Chicken"),
                    ExpiryDate = now + TimeSpan.FromDays(180),
                    Quantity = 4540,
                    Location = dbContext.Locations.First(x => x.LocationName == "Deep Freezer"),
                });

                _ = dbContext.LocationFoods.Add(new LocationFoods()
                {
                    Item = dbContext.Items.First(x => x.Upc == "200BBQ"),
                    ExpiryDate = now + TimeSpan.FromDays(180),
                    Quantity = 220,
                    Location = defaultLocation,
                });

                _ = dbContext.SaveChanges();
            }
        }

        public void PopulateLocations()
        {
            using (var dbContext = new DataBase())
            {
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM Locations;");
                _ = dbContext.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'Locations';");
                _ = dbContext.Locations.Add(new() { LocationName = "Default" });
                _ = dbContext.Locations.Add(new() { LocationName = "Fridge" });
                _ = dbContext.Locations.Add(new() { LocationName = "Freezer" });
                _ = dbContext.Locations.Add(new() { LocationName = "Deep Freezer" });
                _ = dbContext.SaveChanges();
            }
        }

        public void PopulateEquipment()
        {
            using (var dbContext = new DataBase())
            {
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM Equipments;");
                _ = dbContext.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'Equipments';");

                _ = dbContext.Equipments.Add(new() { EquipmentName = "Bread Machine", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                _ = dbContext.Equipments.Add(new() { EquipmentName = "Human", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                _ = dbContext.Equipments.Add(new() { EquipmentName = "Fridge", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                _ = dbContext.Equipments.Add(new Equipment() { EquipmentName = "Sous Vide", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                _ = dbContext.SaveChanges();
            }
        }

        public void PopulateRecipes()
        {
            using (var dbContext = new DataBase())
            {
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM Recipes;");
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM RecipeSteps;");
                _ = dbContext.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'Recipes';");
                _ = dbContext.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'RecipeSteps';");


                var recipe = dbContext.Recipes.Add(new Recipe()
                {
                    Description = "Cooked Chicken",
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() { Amount = 120, Food = dbContext.Foods.First(x=>x.FoodName=="Frozen Chicken") },
                        new() { Amount = 1,  Food = dbContext.Foods.First(x=>x.FoodName=="BBQ Sauce") },
                        new() { Amount = -120,  Food = dbContext.Foods.First(x=>x.FoodName=="Cooked Chicken") },
                    },
                    RecipeSteps = new List<RecipeStep>()
                });
                _ = dbContext.SaveChanges();

                recipe.Entity.RecipeSteps.Add(new RecipeStep()
                {
                    RecipeId = recipe.Entity.RecipeId,
                    Instruction = "Put chicken in Sous Vide.",
                    Order = 1,
                    TimeCost = 1,
                    RecipeStepEquipment = new List<RecipeStepEquipment>()
                    {
                        new (){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"), RecipeStepId  = 2},
                        new (){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Human"), RecipeStepId  = 2},
                    },
                });

                _ = dbContext.SaveChanges();
                recipe.Entity.RecipeSteps.Add(new RecipeStep()
                {
                    RecipeId = recipe.Entity.RecipeId,
                    Instruction = "Let it cook.",
                    TimeCost = 120,
                    RecipeStepEquipment = new List<RecipeStepEquipment>()
                    {
                        new (){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"), RecipeStepId  = 2},
                    }
                });

                _ = dbContext.SaveChanges();
                recipe.Entity.RecipeSteps.Add(new RecipeStep()
                {
                    RecipeId = recipe.Entity.RecipeId,
                    Instruction = "Take chicken out.",
                    TimeCost = 1,
                    RecipeStepEquipment = new List<RecipeStepEquipment>()
                    {
                        new (){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"), RecipeStepId  = 2},
                        new (){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Human"), RecipeStepId  = 2},
                    }
                });
                _ = dbContext.SaveChanges();
            }
        }

        public void PopulateFoods()
        {
            using (var dbContext = new DataBase())
            {
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM Foods;");
                _ = dbContext.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'Foods';");
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Frozen Chicken" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Raw Chicken", });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "BBQ Sauce" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Cooked Chicken" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Sliced Chicken" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Flour" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Eggs" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Milk" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Bread" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Sliced Bread" });
                _ = dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Chicken Sandwich" });
                _ = dbContext.SaveChanges();
            }
        }

    }
}