using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;

namespace Pantry.ServiceGateways
{
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

        public bool AddItem(Core.Models.Food selectedFood, string newItemUpc, double newItemWeight)
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

        public List<Core.Models.Food> GetFoods()
        {
            using (var db = _dbFactory())
            {
                return db.Foods.ToList();
            }
        }

        public void AddLocationFood(Item selectedItem, Core.Models.Location selectedLocation = null)
        {
            using (var db = _dbFactory())
            {
                if (selectedLocation is null)
                {
                    if (!db.Locations.Any()) db.Locations.Add(new() { LocationName = "default" });
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
            using (var db = _dbFactory())
            {
                return db.LocationFoods
                    .Where(x => x.Location.LocationId == selectedLocationId)
                    .Where(x => x.Quantity > 0)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Food)
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

        public List<Core.Models.Location> GetLocations()
        {
            using (var db = _dbFactory())
            {
                return db.Locations.ToList();
            }
        }
    }
}
