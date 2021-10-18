using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;

namespace ServiceGateways
{
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

        public List<Food> GetFoods()
        {
            return _database.Foods.ToList();
        }

        public List<Recipe> GetRecipes()
        {
            using (var db = new DataBase())
            {
                return db.Recipes.ToList();
            }
        }

        public void AddEmptyRecipe(string newRecipeName)
        {
            using (var db = new DataBase())
            {
                db.Recipes.Add(new() { Description = newRecipeName });

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
