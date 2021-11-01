using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;

namespace Pantry.ServiceGateways.Recipe
{
    public class RecipeServiceGateway
    {
        private readonly Func<DataBase> _dbFactory;

        public RecipeServiceGateway(Func<DataBase> dbFactory)
        {
            _dbFactory = dbFactory;
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

        public void AddEmptyRecipe(string newRecipeName)
        {
            using (var db = _dbFactory())
            {
                db.Recipes.Add(new() { Description = newRecipeName });
                db.SaveChanges();
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

        public List<Core.Models.Food> GetFoods()
        {
            using (var db = _dbFactory())
            {
                return db.Foods.ToList();
            }
        }

        public void DeleteRecipe(Core.Models.Recipe recipe)
        {
            using (var db = _dbFactory())
            {
                db.Recipes.Remove(recipe);
                db.SaveChanges();
            }
        }

        public void DeleteFood(RecipeFood selectedRecipeFood)
        {
            using (var db = _dbFactory())
            {
                db.RecipeFoods.Remove(selectedRecipeFood);
                db.SaveChanges();
            }
        }

        public void DeleteRecipeStep(RecipeStep selectedRecipeStep)
        {
            using (var db = _dbFactory())
            {
                db.RecipeSteps.Remove(selectedRecipeStep);
                db.SaveChanges();
            }
        }
        public List<RecipeStep> GetRecipeSteps(int selectedRecipeId)
        {
            using (var db = _dbFactory())
            {
                List<RecipeStep> newList;
                if (!db.RecipeSteps.Any() ||
                     !db.RecipeSteps.Any(x =>
                         x.RecipeId == selectedRecipeId))
                {
                    newList = new();
                }
                else
                {
                    newList = db.RecipeSteps.Where(
                            x => x.RecipeId == selectedRecipeId)
                        .Include(y => y.RecipeStepEquipmentType)
                        .ThenInclude(x => x.EquipmentType)
                        .ToList();
                }

                return newList;
            }
        }

        public List<RecipeFood> GetRecipeFoods(Core.Models.Recipe selectedRecipe)
        {
            using (var db = _dbFactory())
            {
                if (selectedRecipe is null || db.RecipeFoods is null) return null;
                var newList = db.RecipeFoods.Include(x => x.Food).Where(x => x.RecipeId == selectedRecipe.RecipeId)
                    .ToList();

                return newList;
            }
        }

        public bool AddRecipeFood(int recipeId, int foodId, double amount)
        {
            using (var db = _dbFactory())
            {
                var rx =
                    db.Recipes
                        .Where(x => x.RecipeId == recipeId)
                        .Include(x => x.RecipeFoods)
                        .FirstOrDefault();
                if (rx?.RecipeFoods is null)
                {
                    return false;
                }
                var x = new RecipeFood() { Amount = amount, FoodId = foodId, RecipeId = recipeId };
                rx.RecipeFoods.Add(x);
                db.SaveChanges();
                return true;
            }
        }

        public void AddRecipeStep(string newDescription, int recipeId, double tempDuration, IEnumerable<int> equipmentTypeIds)
        {

            using (var db = _dbFactory())
            {
                var entity = db.RecipeSteps.Add(new()
                {
                    Instruction = newDescription,
                    RecipeId = recipeId,
                    TimeCost = tempDuration
                }).Entity;
                var stepId = entity.RecipeStepId;

                entity.RecipeStepEquipmentType = new List<RecipeStepEquipmentType>(equipmentTypeIds
                    .Select(x => new RecipeStepEquipmentType()
                    {
                        EquipmentTypeId = x,
                        RecipeStepId = stepId,
                        EquipmentId = 1,
                    }));
                db.SaveChanges();
            }
        }

        public List<EquipmentTypeProjection> GetEquipmentTypeProjections()
        {
            using (var db = _dbFactory())
            {
                return db.EquipmentTypes
                    .Select(x =>
                        new EquipmentTypeProjection()
                        {
                            EquipmentTypeId = x.EquipmentTypeId,
                            EquipmentTypeName = x.EquipmentTypeName,
                            IsSelected = false,
                        }).ToList();
            }
        }
    }
}