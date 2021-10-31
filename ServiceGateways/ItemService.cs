using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;

namespace Pantry.ServiceGateways
{

    public class EquipmentProjection
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ItemService
    {
        private readonly Func<DataBase> _dbFactory;

        public ItemService(Func<DataBase> dbFactory)
        {
            _dbFactory = dbFactory;
        }



        public List<Item> GetItems()
        {
            using (var db = _dbFactory())
            {
                return db.Items.Include(x => x.Food).ToList();
            }
        }

        public Item GetItem(string upc)
        {
            using (var db = _dbFactory())
            {
                return db.Items.FirstOrDefault(x => x.Upc == upc);
            }
        }

        public bool AddItem(Food selectedFood, string newItemUpc, double newItemWeight)
        {
            using (var db = _dbFactory())
            {
                if (db.Items.Any(x => x.Upc == ""))
                {
                    return false;
                }

                db.Items.Add(new()
                {
                    FoodId = selectedFood.FoodId,
                    Unit = null,
                    Upc = newItemUpc,
                    Weight = newItemWeight
                });
                db.SaveChanges();

                return true;
            }
        }






        public void AddNewLocationFood(CookPlan canCook, Item itemToUse)
        {
            using (var db = _dbFactory())
            {
                db.LocationFoods.Add(new()
                {
                    Exists = true,
                    ExpiryDate = DateTime.Now,
                    Quantity = canCook.TotalOutput.First().Amount,
                    Location = db.Locations.First(),
                    OpenDate = DateTime.MinValue,
                    PurchaseDate = DateTime.MinValue,
                    Item = itemToUse
                });
                db.SaveChanges();
            }
        }



        public List<Core.Models.Recipe> GetRecipes()
        {
            using (var db = _dbFactory())
            {
                return db.Recipes
                    .Include(x => x.RecipeFoods)
                    .ThenInclude(x => x.Food)
                    .ToList();
            }
        }



        public List<Core.Models.Recipe> GetRecipe(int recipeId)
        {
            using (var db = _dbFactory())
            {
                return db.Recipes
                    .Where(x => x.RecipeId == recipeId)
                    .Include(x => x.RecipeFoods)
                    .ThenInclude(x => x.Food)
                    .ToList();
            }
        }


        public void DeleteFood(int foodId)
        {
            using (var db = _dbFactory())
            {
                var x = db.Foods.First(x => x.FoodId == foodId);
                db.Foods.Remove(x);
                db.SaveChanges();
            }
        }


        public List<Food> GetFoods()
        {
            using (var db = _dbFactory())
            {
                return db.Foods.ToList();
            }
        }



        public void AddLocationFood(Item selectedItem, Location selectedLocation = null)
        {
            using (var db = _dbFactory())
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

        public List<LocationFoods> GetLocationFoodsAtLocation(Location locations)
        {
            if (locations is null) return new();

            using (var db = _dbFactory())
            {
                return db.LocationFoods
                    .Where(x => x.Location.LocationId == locations.LocationId)
                    .Where(x => x.Quantity > 0)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Food)
                    .Include(x => x.Location)
                    .ToList();
            }
        }

       


        public List<LocationFoods> GetLocationFoodsAtLocation(int selectedLocationId)
        {
            using (var db = _dbFactory())
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
                using (var db = _dbFactory())
                {
                    var itemToUse = db.Items.Add(new()
                    {
                        FoodId = canCook.TotalOutput.OrderBy(x => x.Amount).First().Food.FoodId,
                        Unit = null,
                        Upc = upc,
                        Weight = canCook.TotalOutput.First().Amount
                    }).Entity;
                    db.SaveChanges();
                    return itemToUse;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }
        }

        public List<LocationFoods> GetLocationFood(int locationFoodsId)
        {
            using (var db = _dbFactory())
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



        public void SaveLocationFood(LocationFoods lf)
        {
            using (var db = _dbFactory())
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
            using (var db = _dbFactory())
            {
                return db.Locations.ToList();
            }
        }
    }
}
