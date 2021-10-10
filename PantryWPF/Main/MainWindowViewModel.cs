using System;
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
            RecipeNavigationCommand = new(this, new RecipesListViewModel());
            InventoryNavigationCommand = new(this, new InventoryViewModel());
            FoodNavigationCommand = new(this, new FoodListViewModel());
            SeedDatabaseCommand = new(SeedDatabase);
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
                _ = dbContext.Items.Add(new() { Food = dbContext.Foods.First(x => x.FoodName == "Frozen Chicken"), Weight = 4540, Upc = "100Chicken" });
                _ = dbContext.Items.Add(new() { Food = dbContext.Foods.First(x => x.FoodName == "BBQ Sauce"), Weight = 454, Upc = "200BBQ" });
                _ = dbContext.Items.Add(new() { Food = dbContext.Foods.First(x => x.FoodName == "Milk"), Weight = 3900, Upc = "1GalMilk" });
                _ = dbContext.Items.Add(new() { Food = dbContext.Foods.First(x => x.FoodName == "Sliced Bread"), Weight = 454, Upc = "LotsOfBread" });
                _ = dbContext.Items.Add(new() { Food = dbContext.Foods.First(x => x.FoodName == "Bread"), Weight = 1000, Upc = "Artisan Bread" });

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

                _ = dbContext.LocationFoods.Add(new()
                {
                    Item = dbContext.Items.First(x => x.Upc == "1GalMilk"),
                    ExpiryDate = now + TimeSpan.FromDays(14),
                    Quantity = 3900,
                    Location = defaultLocation,
                    Exists = true
                });

                _ = dbContext.LocationFoods.Add(new()
                {
                    Item = dbContext.Items.First(x => x.Upc == "Artisan Bread"),
                    ExpiryDate = now + TimeSpan.FromDays(14),
                    Quantity = 1000,
                    Location = defaultLocation,
                    Exists = true
                });

                _ = dbContext.LocationFoods.Add(new()
                {
                    Item = dbContext.Items.First(x => x.Upc == "LotsOfBread"),
                    OpenDate = now + TimeSpan.FromDays(-7),
                    ExpiryDate = now + TimeSpan.FromDays(1),
                    Quantity = 454,
                    Location = defaultLocation,
                    Exists = true
                });

                _ = dbContext.LocationFoods.Add(new()
                {
                    Item = dbContext.Items.First(x => x.Upc == "1GalMilk"),
                    OpenDate = now + TimeSpan.FromDays(-7),
                    ExpiryDate = now + TimeSpan.FromDays(1),
                    Quantity = 3900,
                    Location = defaultLocation,
                    Exists = true
                });

                _ = dbContext.LocationFoods.Add(new()
                {
                    Item = dbContext.Items.First(x => x.Upc == "100Chicken"),
                    ExpiryDate = now + TimeSpan.FromDays(180),
                    Quantity = 75,
                    Location = defaultLocation,
                    Exists = true
                });

                _ = dbContext.LocationFoods.Add(new()
                {
                    Item = dbContext.Items.First(x => x.Upc == "100Chicken"),
                    ExpiryDate = now + TimeSpan.FromDays(180),
                    Quantity = 66,
                    Location = dbContext.Locations.First(x => x.LocationName == "Deep Freezer"),
                    Exists = true
                });

                _ = dbContext.LocationFoods.Add(new()
                {
                    Item = dbContext.Items.First(x => x.Upc == "200BBQ"),
                    ExpiryDate = now + TimeSpan.FromDays(180),
                    Quantity = 220,
                    Location = defaultLocation,
                    Exists = true
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
                _ = dbContext.Equipments.Add(new() { EquipmentName = "Sous Vide", Location = dbContext.Locations.First(x => x.LocationName == "Default") });
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

                /* Cooked Chicken */
                {
                    var recipe = dbContext.Recipes.Add(new()
                    {
                        Description = "Cooked Chicken",
                        RecipeFoods = new()
                        {
                            new() { Amount = 120, Food = dbContext.Foods.First(x => x.FoodName == "Raw Chicken") },
                            new() { Amount = 1, Food = dbContext.Foods.First(x => x.FoodName == "BBQ Sauce") },
                            new() { Amount = -120, Food = dbContext.Foods.First(x => x.FoodName == "Cooked Chicken") },
                        },
                        RecipeSteps = new()
                    });
                    _ = dbContext.SaveChanges();

                    recipe.Entity.RecipeSteps.Add(new()
                    {
                        RecipeId = recipe.Entity.RecipeId,
                        Instruction = "Put chicken in Sous Vide.",
                        Order = 1,
                        TimeCost = 1,
                        RecipeStepEquipment = new List<RecipeStepEquipment>()
                        {
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"),
                                RecipeStepId = 2
                            },
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Human"),
                                RecipeStepId = 2
                            },
                        },
                    });

                    _ = dbContext.SaveChanges();
                    recipe.Entity.RecipeSteps.Add(new()
                    {
                        RecipeId = recipe.Entity.RecipeId,
                        Instruction = "Let it cook.",
                        Order = 2,
                        TimeCost = 120,
                        RecipeStepEquipment = new List<RecipeStepEquipment>()
                        {
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"),
                                RecipeStepId = 2
                            },
                        }
                    });

                    _ = dbContext.SaveChanges();
                    recipe.Entity.RecipeSteps.Add(new()
                    {
                        RecipeId = recipe.Entity.RecipeId,
                        Instruction = "Take chicken out.",
                        Order = 3,
                        TimeCost = 1,
                        RecipeStepEquipment = new List<RecipeStepEquipment>()
                        {
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Sous Vide"),
                                RecipeStepId = 2
                            },
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Human"),
                                RecipeStepId = 2
                            },
                        }
                    });
                }

                /* Raw Chicken */
                {
                    var recipe = dbContext.Recipes.Add(new()
                    {
                        Description = "Raw Chicken",
                        RecipeFoods = new()
                        {
                            new() { Amount = 120, Food = dbContext.Foods.First(x => x.FoodName == "Frozen Chicken") },
                            new() { Amount = -120, Food = dbContext.Foods.First(x => x.FoodName == "Raw Chicken") },
                        },
                        RecipeSteps = new()
                    });
                    _ = dbContext.SaveChanges();

                    recipe.Entity.RecipeSteps.Add(new()
                    {
                        RecipeId = recipe.Entity.RecipeId,
                        Instruction = "Put chicken in Fridge.",
                        Order = 1,
                        TimeCost = 1,
                        RecipeStepEquipment = new List<RecipeStepEquipment>()
                        {
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Fridge"),
                                RecipeStepId = 2
                            },
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Human"),
                                RecipeStepId = 2
                            },
                        },
                    });

                    _ = dbContext.SaveChanges();
                    recipe.Entity.RecipeSteps.Add(new()
                    {
                        RecipeId = recipe.Entity.RecipeId,
                        Instruction = "Let it defrost.",
                        Order = 2,
                        TimeCost = 120,
                        RecipeStepEquipment = new List<RecipeStepEquipment>()
                        {
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Fridge"),
                                RecipeStepId = 2
                            },
                        }
                    });
                }

                /* Sliced Bread */
                {
                    var recipe = dbContext.Recipes.Add(new()
                    {
                        Description = "Sliced Bread",
                        RecipeFoods = new()
                        {
                            new() { Amount = 454, Food = dbContext.Foods.First(x => x.FoodName == "Bread") },
                            new() { Amount = -454, Food = dbContext.Foods.First(x => x.FoodName == "Sliced Bread") },
                        },
                        RecipeSteps = new()
                    });
                    _ = dbContext.SaveChanges();

                    recipe.Entity.RecipeSteps.Add(new()
                    {
                        RecipeId = recipe.Entity.RecipeId,
                        Instruction = "Cut Bread.",
                        Order = 1,
                        TimeCost = 3,
                        RecipeStepEquipment = new List<RecipeStepEquipment>()
                        {
                            new()
                            {
                                Equipment = dbContext.Equipments.First(x => x.EquipmentName == "Human"),
                                RecipeStepId = 1
                            },
                        },
                    });
                }

                _ = dbContext.SaveChanges();
            }
        }

        public void PopulateFoods()
        {
            using (var dbContext = new DataBase())
            {
                _ = dbContext.Database.ExecuteSqlRaw("DELETE FROM Foods;");
                _ = dbContext.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'Foods';");
                _ = dbContext.Foods.Add(new() { FoodName = "Frozen Chicken" });
                _ = dbContext.Foods.Add(new() { FoodName = "Raw Chicken", });
                _ = dbContext.Foods.Add(new() { FoodName = "BBQ Sauce" });
                _ = dbContext.Foods.Add(new() { FoodName = "Cooked Chicken" });
                _ = dbContext.Foods.Add(new() { FoodName = "Sliced Chicken" });
                _ = dbContext.Foods.Add(new() { FoodName = "Flour" });
                _ = dbContext.Foods.Add(new() { FoodName = "Eggs" });
                _ = dbContext.Foods.Add(new() { FoodName = "Milk" });
                _ = dbContext.Foods.Add(new() { FoodName = "Bread" });
                _ = dbContext.Foods.Add(new() { FoodName = "Sliced Bread" });
                _ = dbContext.Foods.Add(new() { FoodName = "Chicken Sandwich" });
                _ = dbContext.SaveChanges();
            }
        }

    }
}