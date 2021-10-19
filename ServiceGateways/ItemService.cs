using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;

namespace ServiceGateways
{

    public class EquipmentProjection
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public bool IsSelected { get; set; }
    }


    public class ItemService
    {
        private readonly DataBase _database;

        public ItemService()
        {
            _database = new();
        }

        public List<Item> GetItems()
        {
            return _database.Items.Include(x => x.Food).ToList();
        }

        public Item GetItem(string upc)
        {
            using (var db = new DataBase())
            {
                return db.Items.FirstOrDefault(x => x.Upc == upc);
            }
        }

        public bool AddItem(Food selectedFood, string newItemUpc, double newItemWeight)
        {
            using (var db = new DataBase())
            {
                if (db.Items.Any(x => x.Upc == "")) { return false; }

                db.Items.Add(new()
                {
                    FoodId = selectedFood.FoodId,
                    Unit = null,
                    Upc = newItemUpc,
                    Weight = newItemWeight
                });
                db.SaveChanges();
            }
            return true;
        }


        public List<Recipe> GetRecipes()
        {
            using (var db = new DataBase())
            {
                return db.Recipes
                    .Include(x => x.RecipeFoods)
                    .ThenInclude(x => x.Food)
                    .ToList();
            }
        }

        public List<Equipment> GetEquipments()
        {
            using (var db = new DataBase())
            {
                return db.Equipments.ToList();
            }
        }

        public List<EquipmentProjection> GetEquipmentProjections()
        {
            using (var db = new DataBase())
            {
                return db.Equipments.Select(x =>
                    new EquipmentProjection()
                    {
                        IsSelected = false,
                        EquipmentId = x.EquipmentId,
                        EquipmentName = x.EquipmentName
                    }).ToList();
            }
        }

        public List<Recipe> GetRecipe(int recipeId)
        {
            using (var db = new DataBase())
            {
                return db.Recipes
                    .Where(x => x.RecipeId == recipeId)
                    .Include(x => x.RecipeFoods)
                    .ThenInclude(x => x.Food)
                    .ToList();
            }
        }

        public void DeleteRecipe(Recipe recipe)
        {
            using (var db = new DataBase())
            {
                db.Recipes.Remove(recipe);
                db.SaveChanges();
            }
        }

        public void DeleteFood(RecipeFood selectedRecipeFood)
        {
            using (var db = new DataBase())
            {
                db.RecipeFoods.Remove(selectedRecipeFood);
                db.SaveChanges();
            }
        }

        public void DeleteRecipeStep(RecipeStep selectedRecipeStep)
        {
            using (var db = new DataBase())
            {
                db.RecipeSteps.Remove(selectedRecipeStep);
                db.SaveChanges();
            }
        }


        public List<Food> GetFoods()
        {
            return _database.Foods.ToList();
        }

        public void AddEmptyRecipe(string newRecipeName)
        {
            using (var db = new DataBase())
            {
                db.Recipes.Add(new() { Description = newRecipeName });

            }
        }

        public void AddLocationFood(Item selectedItem, Location selectedLocation = null)
        {

            using (var db = new DataBase())
            {
                if (selectedLocation is null)
                {
                    selectedLocation = db.Locations.First();
                }

                db.LocationFoods.Add(new()
                {
                    Exists = true,
                    ExpiryDate = DateTime.MinValue,
                    OpenDate = DateTime.MinValue,
                    ItemId = selectedItem.ItemId,
                    LocationId = selectedLocation.LocationId,
                    Quantity = db.Items.Single(x => x.FoodId == selectedItem.FoodId).Weight
                });
                db.SaveChanges();
            }
        }

        public List<LocationFoods> GetLocationFoodsAtLocation(int selectedLocationId)
        {
            using (var db = new DataBase())
            {
                return db.LocationFoods
                    .Where(x => x.Location.LocationId == selectedLocationId)
                    .Where(x => x.Quantity > 0)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Food)
                    .Include(x => x.Location)
                    .ToList();
            }
        }

        public Item AddSomething(CookPlan canCook)
        {
            var upc = canCook.TotalOutput.OrderBy(x => x.Amount).First().Food.FoodName;
            try
            {
                using (var db = new DataBase())
                {
                    var itemToUse = db.Items.Add(new()
                    {
                        FoodId = canCook.TotalOutput.OrderBy(x => x.Amount).First().Food.FoodId,
                        Unit = null,
                        Upc = upc,
                        Weight = canCook.TotalOutput.First().Amount
                    }).Entity;
                    return itemToUse;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<LocationFoods> GetLocationFood(int locationFoodsId)
        {
            using (var db = new DataBase())
            {
                return db.LocationFoods
                    .Where(x => x.LocationFoodsId == locationFoodsId)
                    .Where(x => x.Quantity > 0)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Food)
                    .Include(x => x.Location)
                    .ToList();
            }
        }



        public List<LocationFoods> GetLocationFoods()
        {
            using (var db = new DataBase())
            {
                return db.LocationFoods
                    .Where(x => x.Quantity > 0)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Food)
                    .Include(x => x.Location)
                    .ToList();
            }
        }

        public void SaveLocationFood(LocationFoods lf)
        {
            using (var db = new DataBase())
            {
                var x =
                    db.LocationFoods.First(x => x.LocationFoodsId == lf.LocationFoodsId);
                x.Quantity = lf.Quantity;
                x.LocationId = lf.Location.LocationId;
                db.SaveChanges();

            }
        }

        public List<Location> GetLocations()
        {
            using (var db = new DataBase())
            {
                return db.Locations.ToList();
            }

        }


    }
}
