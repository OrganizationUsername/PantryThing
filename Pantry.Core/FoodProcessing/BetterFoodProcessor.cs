using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Models;

namespace Pantry.Core.FoodProcessing
{
    public class BetterFoodProcessor
    {
        public CookPlan GetCookPlan(IList<RecipeFood> foodInventory, Recipe recipe, IList<Recipe> recipes)
        {
            var totalOutput = new List<RecipeFood>();
            var totalInput = new List<RecipeFood>();
            var clonedFoodInventory = CloneFoodInstances(foodInventory);
            var recipeLines = GetFoodInstancesFromRecipe(recipe);
            RecipeDag recipeDag = new() { MainRecipe = recipe };
            foreach (var foodInstance in recipeLines)
            {
                var onlyPantryUsed = true;
                while (foodInstance.Amount > 0)
                {
                    var target = clonedFoodInventory.OrderBy(x => x.Amount).FirstOrDefault(x => x.Food.FoodId == foodInstance.Food.FoodId && x.Amount > 0);
                    if (target is not null)
                    {
                        var diminishAmount = Math.Min(foodInstance.Amount, target.Amount);
                        foodInstance.Amount -= diminishAmount;
                        target.Amount -= diminishAmount;
                        if (onlyPantryUsed)
                        {
                            totalInput.Add(new() { Amount = diminishAmount, Food = foodInstance.Food });
                        }
                        else
                        {
                            var outputToReduce = totalOutput.First(x => x.Food.FoodId == foodInstance.Food.FoodId && x.Amount > 0);
                            outputToReduce.Amount -= diminishAmount;
                        }
                        continue;
                    }
                    onlyPantryUsed = false;
                    var newRecipe = RecipeFinder(foodInstance.Food.FoodId, recipes);

                    if (newRecipe is null)
                    {
                        return new() { CanMake = false };
                    }

                    var result = this.GetCookPlan(CloneFoodInstances(clonedFoodInventory), newRecipe, recipes);
                    if (result.CanMake)
                    {
                        recipeDag.SubordinateBetterRecipes.Add(result.RecipeDag);
                        totalInput.AddRange(result.TotalInput.Where(x => x.Amount > 0));
                        clonedFoodInventory.AddRange(CloneFoodInstances(result.TotalOutput));
                        totalOutput.AddRange(CloneFoodInstances(result.TotalOutput));
                    }
                    else
                    {
                        return new() { CanMake = false };
                    }
                }
            }
            totalOutput.AddRange(recipe.RecipeFoods.Where(x => x.Amount < 0).Select(x => new RecipeFood() { Amount = -x.Amount, Food = x.Food }));

            return new()
            {
                CanMake = true,
                TotalOutput = totalOutput,
                TotalInput = totalInput,
                RecipeDag = recipeDag
            };
        }

        public static Recipe RecipeFinder(int FoodId, IList<Recipe> recipes)
        {
            Recipe? x = null;
            foreach (var r in recipes)
            {
                if (r.RecipeFoods is not null && r.RecipeFoods.Any(fi => fi.Food.FoodId == FoodId && fi.Amount < 0))
                {
                    x = r;
                    break;
                }
            }

            return x;
        }

        private static RecipeFood[] GetFoodInstancesFromRecipe(Recipe recipe)
        {
            var clones = new RecipeFood[recipe.RecipeFoods.Count];
            for (var index = 0; index < recipe.RecipeFoods.Count; index++)
            {
                var fi = recipe.RecipeFoods[index];
                clones[index] = (new()
                {
                    Food = fi.Food,
                    Amount = fi.Amount
                });
            }
            return clones;
        }

        private static List<RecipeFood> CloneFoodInstances(IList<RecipeFood> foodInstances)
        {
            var clones = new RecipeFood[foodInstances.Count];
            for (var index = 0; index < foodInstances.Count; index++)
            {
                var fi = foodInstances[index];
                clones[index] = (new()
                {
                    Food = new() { FoodName = fi.Food.FoodName, FoodId = fi.Food.FoodId },
                    Amount = fi.Amount
                });
            }
            return clones.ToList();
        }
    }
}