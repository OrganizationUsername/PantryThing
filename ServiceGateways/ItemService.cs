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

        public void AddLocationFood(Item selectedItem)
        {
            using (var db = new DataBase())
            {
                db.LocationFoods.Add(new()
                {
                    Exists = true,
                    ExpiryDate = DateTime.MinValue,
                    ItemId = selectedItem.ItemId,
                    Location = db.Locations.First(),
                    OpenDate = DateTime.MinValue,
                    Quantity = selectedItem.Weight
                });
                db.SaveChanges();
            }
        }


    }
}
