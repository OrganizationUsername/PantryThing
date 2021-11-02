using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pantry.Data;
using Serilog.Core;

namespace Pantry.ServiceGateways.Food
{
    public class FoodServiceGateway
    {
        private readonly Func<DataBase> _dbFactory;
        private readonly Logger _logger;

        public FoodServiceGateway(Func<DataBase> dbFactory, Logger logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task DeleteFood(int foodId)
        {
            using (var db = _dbFactory())
            {
                var x = await db.Foods.FirstAsync(x => x.FoodId == foodId);
                db.Foods.Remove(x);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Core.Models.Food>> GetAllFoods()
        {
            using (var db = _dbFactory())
            {
                if (db.Foods is null || db.Foods.FirstOrDefaultAsync() is null) return null;
                return await db.Foods
                    .Include(x => x.RecipeFoods)
                    .ThenInclude(x => x.Recipe).ToListAsync();
            }
        }

        public async Task<bool> SaveSelectedFood(int foodId, bool isEdible, string foodName)
        {
            using (var db = _dbFactory())
            {
                var food = await db.Foods.FirstOrDefaultAsync(x => x.FoodId == foodId);
                if (food is null) return false;
                food.FoodName = foodName;
                food.IsEdible = isEdible;
                return 1 == await db.SaveChangesAsync();
            }
        }

        public async Task AddFood(string newFoodName)
        {
            using (var db = _dbFactory())
            {
                var x = await db.Foods.AddAsync(new() { FoodName = newFoodName });
                var rowCount = await db.SaveChangesAsync();
                if (rowCount != 1) { _logger.Warning($"{rowCount} rows modified. Should be exactly 1."); }
                _logger.Debug($"Added {x.Entity.FoodName} with ID= {x.Entity.FoodId}.");
            }
        }

        public async Task KeepOnlyUniqueFoodNames()
        {
            //ToDo: This method should be a part of another Service Gateway that finds issues.
            using (var db = _dbFactory())
            {
                var names = new List<string>();
                foreach (var x in db.Foods)
                {
                    if (!names.Contains(x.FoodName) && !string.IsNullOrWhiteSpace(x.FoodName))
                    {
                        names.Add(x.FoodName);
                    }
                    else
                    {
                        db.Foods.Remove(x);
                    }
                }
                await db .SaveChangesAsync();
            }
        }

        public async Task<List<Core.Models.Recipe>> GetRecipes(Core.Models.Food selectedFood)
        {
            using (var db = _dbFactory())
            {
                if (selectedFood is null || db.RecipeFoods is null) return null;

                var newList = await db.RecipeFoods 
                    .Where(x => x.FoodId == selectedFood.FoodId)
                    .Select(x => x.Recipe)
                    .ToListAsync();

                return newList;
            }
        }

    }
}