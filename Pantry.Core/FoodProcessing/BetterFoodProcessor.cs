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
                    var target = clonedFoodInventory.FirstOrDefault(x => x.Food.FoodId == foodInstance.Food.FoodId && x.Amount > 0);
                    if (target is not null)
                    {
                        var diminishAmount = Math.Min(foodInstance.Amount, target.Amount);
                        foodInstance.Amount -= diminishAmount;
                        target.Amount -= diminishAmount;
                        if (onlyPantryUsed)
                        {
                            totalInput.Add(new RecipeFood() { Amount = diminishAmount, Food = foodInstance.Food });
                        }
                        else
                        {
                            var outputToReduce = totalOutput.First(x => x.Food.FoodId == foodInstance.Food.FoodId && x.Amount > 0);
                            outputToReduce.Amount -= diminishAmount;
                        }
                        continue;
                    }
                    onlyPantryUsed = false;
                    var newRecipe = recipes.FirstOrDefault(x => x.MainOutput.FoodId == foodInstance.Food.FoodId);
                    if (newRecipe is null)
                    {
                        return new CookPlan() { CanMake = false };
                    }

                    var result = this.GetCookPlan(CloneFoodInstances(clonedFoodInventory), newRecipe, recipes);
                    if (result.CanMake)
                    {
                        recipeDag.SubordinateBetterRecipes.Add(result.RecipeDag);
                        totalInput.AddRange(result.TotalInput);
                        clonedFoodInventory.AddRange(CloneFoodInstances(result.TotalOutput));
                        totalOutput.AddRange(CloneFoodInstances(result.TotalOutput));
                    }
                    else
                    {
                        return new CookPlan() { CanMake = false };
                    }
                }
            }
            totalOutput.AddRange(recipe.Outputs);

            return new CookPlan()
            {
                CanMake = true,
                TotalOutput = totalOutput,
                TotalInput = totalInput,
                RecipeDag = recipeDag
            };
        }

        private static RecipeFood[] GetFoodInstancesFromRecipe(Recipe recipe)
        {
            RecipeFood[] clones = new RecipeFood[recipe.Inputs.Count];
            for (var index = 0; index < recipe.Inputs.Count; index++)
            {
                RecipeFood fi = recipe.Inputs[index];
                clones[index] = (new RecipeFood()
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
                RecipeFood fi = foodInstances[index];
                clones[index] = (new RecipeFood()
                {
                    Food = new Food() { Name = fi.Food.Name, FoodId = fi.Food.FoodId },
                    Amount = fi.Amount
                });
            }
            return clones.ToList();
        }
    }
}