using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Models;
using Pantry.Core.Models;

namespace Pantry.Core.FoodProcessing
{
    public class BetterFoodProcessor
    {
        public CookPlan GetCookPlan(IList<FoodInstance> foodInventory, BetterRecipe recipe, IList<BetterRecipe> recipes)
        {
            var totalOutput = new List<FoodInstance>();
            var totalInput = new List<FoodInstance>();
            var clonedFoodInventory = CloneFoodInstances(foodInventory);
            var recipeLines = GetFoodInstancesFromRecipe(recipe);
            RecipeDAG recipeDag = new() { MainRecipe = recipe };
            foreach (var foodInstance in recipeLines)
            {
                bool onlyPantryUsed = true;
                while (foodInstance.Amount > 0)
                {
                    var target = clonedFoodInventory.FirstOrDefault(x => x.FoodType.FoodId == foodInstance.FoodType.FoodId && x.Amount > 0);
                    if (target is not null)
                    {
                        var diminishAmount = Math.Min(foodInstance.Amount, target.Amount);
                        foodInstance.Amount -= diminishAmount;
                        target.Amount -= diminishAmount;
                        if (onlyPantryUsed)
                        {
                            totalInput.Add(new FoodInstance() { Amount = diminishAmount, FoodType = foodInstance.FoodType });
                        }
                        else
                        {
                            var outputToReduce = totalOutput.First(x => x.FoodType.FoodId == foodInstance.FoodType.FoodId && x.Amount > 0);
                            outputToReduce.Amount -= diminishAmount;
                        }
                        continue;
                    }
                    onlyPantryUsed = false;
                    var newRecipe = recipes.FirstOrDefault(x => x.MainOutput.FoodId == foodInstance.FoodType.FoodId);
                    if (newRecipe is null)
                    {
                        return new CookPlan() { CanMake = false };
                    }

                    var result = this.GetCookPlan(CloneFoodInstances(clonedFoodInventory), newRecipe, recipes);
                    if (result.CanMake)
                    {
                        recipeDag.SubordinateBetterRecipes.Add(result.RecipeDAG);
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
                RecipeDAG = recipeDag
            };
        }
        private static FoodInstance[] GetFoodInstancesFromRecipe(BetterRecipe recipe)
        {
            FoodInstance[] clones = new FoodInstance[recipe.Inputs.Count];
            for (var index = 0; index < recipe.Inputs.Count; index++)
            {
                FoodInstance fi = recipe.Inputs[index];
                clones[index] = (new FoodInstance()
                {
                    FoodType = fi.FoodType,
                    Amount = fi.Amount
                });
            }
            return clones;
        }
        private static List<FoodInstance> CloneFoodInstances(IList<FoodInstance> foodInstances)
        {
            var clones = new FoodInstance[foodInstances.Count];
            for (var index = 0; index < foodInstances.Count; index++)
            {
                FoodInstance fi = foodInstances[index];
                clones[index] = (new FoodInstance()
                {
                    FoodType = new Food() { Name = fi.FoodType.Name, FoodId = fi.FoodType.FoodId },
                    Amount = fi.Amount
                });
            }
            return clones.ToList();
        }


    }

}