using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

            using (var dbContext = new DataBase())
            {
                var existingFoods = dbContext.Foods.Select(x => x.FoodName).ToList();
                var x = dbContext.Recipes.ToListAsync();
            }
        }

        public void SeedDatabase()
        {
            PopulateFoods();
            PopulateLocations();
            PopulateEquipment();
            PopulateRecipes();
        }

        public void PopulateLocations()
        {
            using (var dbContext = new DataBase())
            {
                dbContext.Database.ExecuteSqlRaw("DELETE FROM Locations;");
                dbContext.Locations.Add(new() { LocationName = "Default" });
                dbContext.SaveChanges();
            }
        }

        public void PopulateEquipment()
        {

            using (var dbContext = new DataBase())
            {
                dbContext.Database.ExecuteSqlRaw("DELETE FROM Equipments;");

                dbContext.Equipments.Add(new() { EquipmentName = "Bread Machine", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                dbContext.Equipments.Add(new() { EquipmentName = "Human", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                dbContext.Equipments.Add(new() { EquipmentName = "Fridge", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                dbContext.Equipments.Add(new Equipment() { EquipmentName = "Sous Vide", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
                dbContext.SaveChanges();
            }
        }

        public void PopulateRecipes()
        {

            using (var dbContext = new DataBase())
            {
                dbContext.Database.ExecuteSqlRaw("DELETE FROM Recipes;");
                dbContext.Database.ExecuteSqlRaw("DELETE FROM RecipeSteps;");

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
                var savedrecipe = dbContext.SaveChanges();

                recipe.Entity.RecipeSteps.Add(new RecipeStep()
                {
                    RecipeId = recipe.Entity.RecipeId,
                    Instruction = "Put chicken in Sous Vide.",
                    Order = 1,
                    TimeCost = 1,
                    RecipeStepEquipment = new List<RecipeStepEquipment>()
                    {
                        new RecipeStepEquipment(){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"), RecipeStepId  = 2},
                        new RecipeStepEquipment(){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Human"), RecipeStepId  = 2},
                    },
                });

                var numberadded = dbContext.SaveChanges();
                recipe.Entity.RecipeSteps.Add(new RecipeStep()
                {
                    RecipeId = recipe.Entity.RecipeId,
                    Instruction = "Let it cook.",
                    TimeCost = 120,
                    RecipeStepEquipment = new List<RecipeStepEquipment>()
                    {
                        new RecipeStepEquipment(){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"), RecipeStepId  = 2},
                    }
                });

                numberadded = dbContext.SaveChanges();
                recipe.Entity.RecipeSteps.Add(new RecipeStep()
                {
                    RecipeId = recipe.Entity.RecipeId,
                    Instruction = "Take chicken out.",
                    TimeCost = 1,
                    RecipeStepEquipment = new List<RecipeStepEquipment>()
                    {
                        new RecipeStepEquipment(){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"), RecipeStepId  = 2},
                        new RecipeStepEquipment(){Equipment =dbContext.Equipments.First(x => x.EquipmentName == "Human"), RecipeStepId  = 2},
                    }
                });

                numberadded = dbContext.SaveChanges();

            }

        }

        public void PopulateFoods()
        {
            using (var dbContext = new DataBase())
            {
                dbContext.Database.ExecuteSqlRaw("DELETE FROM Foods;");
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Frozen Chicken" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Raw Chicken", });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "BBQ Sauce" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Cooked Chicken" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Sliced Chicken" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Flour" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Eggs" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Milk" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Bread" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Sliced Bread" });
                dbContext.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Chicken Sandwich" });
                dbContext.SaveChanges();
            }
        }

    }
}