using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public List<MealInstance> GetMealInstances()
        {
            using (var db = _dbFactory())
            {
                return db.MealInstances.Include(x => x.Consumer).ToList();
            }
        }

        public async Task<MealInstance> GetMealInstance(int mealInstanceId)
        {
            using (var db = _dbFactory())
            {
                return await db
                    .MealInstances
                    .Include(x => x.MealInstanceRows).ThenInclude(x => x.Food)
                    .FirstAsync(x => x.MealInstanceId == mealInstanceId);
            }
        }



        public async Task<List<Item>> GetItems()
        {
            using (var db = _dbFactory())
            {
                return await db.Items.Include(x => x.Food).ToListAsync();
            }
        }

        public async Task<bool> AddItem(Core.Models.Food selectedFood, string newItemUpc, double newItemWeight)
        {
            using (var db = _dbFactory())
            {
                if (await db.Items.AnyAsync(x => x.Upc == ""))
                {
                    return false;
                }

                await db .Items.AddAsync(new()
                    {
                        FoodId = selectedFood.FoodId,
                        Unit = null,
                        Upc = newItemUpc,
                        Weight = newItemWeight
                    });
                await db .SaveChangesAsync();

                return true;
            }
        }

        public async Task<List<Core.Models.Food>> GetFoods()
        {
            using (var db = _dbFactory())
            {
                return await db.Foods.ToListAsync();
            }
        }

        public async Task AddLocationFood(Item selectedItem, Core.Models.Location selectedLocation = null)
        {
            using (var db = _dbFactory())
            {
                if (selectedLocation is null)
                {
                    if (!db.Locations.Any()) await db.Locations.AddAsync(new() { LocationName = "default" });
                    selectedLocation = await db.Locations.FirstAsync();
                }

                var item = await db.Items.SingleAsync(x => x.FoodId == selectedItem.FoodId);

                await db.LocationFoods.AddAsync(new()
                {
                    Exists = true,
                    ExpiryDate = DateTime.MinValue,
                    OpenDate = DateTime.MinValue,
                    ItemId = selectedItem.ItemId,
                    LocationId = selectedLocation.LocationId,
                    Quantity = item.Weight
                });

                await db.SaveChangesAsync();
            }
        }

        public async Task<List<LocationFoods>> GetLocationFoodsAtLocation(int selectedLocationId)
        {
            using (var db = _dbFactory())
            {
                return await db.LocationFoods
                    .Where(x => x.Location.LocationId == selectedLocationId)
                    .Where(x => x.Quantity > 0)
                    .Include(x => x.Item)
                    .ThenInclude(x => x.Food)
                    .ToListAsync();
            }
        }

        public async Task SaveLocationFood(LocationFoods lf)
        {
            using (var db = _dbFactory())
            {
                var x =
                   await db.LocationFoods.FirstAsync(x => x.LocationFoodsId == lf.LocationFoodsId);
                x.Quantity = lf.Quantity;
                x.LocationId = lf.Location.LocationId;
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Core.Models.Location>> GetLocations()
        {
            using (var db = _dbFactory())
            {
                return await db.Locations.ToListAsync();
            }
        }
    }
}
