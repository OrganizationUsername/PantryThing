using System;
using System.Collections.Generic;
using System.Linq;
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

        public void DeleteFood(int foodId)
        {
            using (var db = _dbFactory())
            {
                var x = db.Foods.First(x => x.FoodId == foodId);
                db.Foods.Remove(x);
                db.SaveChanges();
            }
        }

        public List<Core.Models.Food> GetAllFoods()
        {
            using (var db = _dbFactory())
            {
                if (db.Foods is null || !db.Foods.Any()) return null;
                return db.Foods
                    .Include(x => x.RecipeFoods)
                    .ThenInclude(x => x.Recipe).ToList();
            }
        }
        public void AddFood(string newFoodName)
        {
            using (var db = _dbFactory())
            {
                var x = db.Foods.Add(new() { FoodName = newFoodName });
                var rowCount = db.SaveChanges();
                if (rowCount != 1) { _logger.Warning($"{rowCount} rows modified. Should be exactly 1."); }
                _logger.Debug($"Added {x.Entity.FoodName} with ID= {x.Entity.FoodId}.");
            }
        }

        public void KeepOnlyUniqueFoodNames()
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
                db.SaveChanges();
            }
        }

        public List<Core.Models.Recipe> GetRecipes(Core.Models.Food selectedFood)
        {
            using (var db = _dbFactory())
            {
                if (selectedFood is null || db.RecipeFoods is null) return null;

                var newList = db.RecipeFoods
                    .Where(x => x.FoodId == selectedFood.FoodId)
                    .Select(x => x.Recipe)
                    .ToList();

                return newList;
            }
        }

    }
}