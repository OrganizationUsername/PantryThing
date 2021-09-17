using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Models;

namespace Pantry.Core.FoodProcessing
{
    /// <summary>
    /// This needs to output a hierarchy of what to cook as well.
    /// This is considered simple because it only considers one recipe at a time.
    /// </summary>
    public class SimpleFoodProcessor : IFoodProcessor
    {
        public CookPlan
            CanCookSomething(IList<RecipeFood> foodInventory, Recipe recipe, IList<Recipe> recipes = null)
        {
            List<Recipe> recipesTouched = new();
            var totalInput = new List<RecipeFood>();
            var rawCost = new List<RecipeFood>();
            var recipeLines = GetFoodInstancesFromRecipe(recipe);
            var clonedFoodInventory = CloneFoodInstances(foodInventory);
            foreach (var recipeInputFoodInstance in recipeLines)
            {
                var usedRaw = true;
                while (recipeInputFoodInstance.Amount > 0)
                {
                    var fi = clonedFoodInventory.FirstOrDefault(foodInstance =>
                        foodInstance.Food == recipeInputFoodInstance.Food
                        && foodInstance.Amount > 0);
                    if (fi is not null /*&& clonedFoodInventory.Where(foodInstance =>
                        foodInstance.Food == recipeInputFoodInstance.Food).Sum(x => x.Amount) >= recipeInputFoodInstance.Amount*/)
                    {
                        double amountUsed = Math.Min(recipeInputFoodInstance.Amount, fi.Amount);
                        recipeInputFoodInstance.Amount -= amountUsed;
                        fi.Amount -= amountUsed;
                        totalInput.Add(new RecipeFood() { Food = fi.Food, Amount = amountUsed });
                        if (usedRaw)
                        {
                            rawCost.Add(new RecipeFood() { Food = recipeInputFoodInstance.Food, Amount = amountUsed });
                        }
                        continue;
                    }
                    usedRaw = false;
                    if (recipes is not null)
                    {
                        var subRecipe = recipes.FirstOrDefault(r =>
                            r.RecipeFoods.First(x=>x.Amount<0).Food == recipeInputFoodInstance.Food
                            && CanCookSomething(clonedFoodInventory, r, recipes).CanMake);
                        if (subRecipe is not null)
                        {
                            var payload = CanCookSomething(clonedFoodInventory, subRecipe, recipes);
                            while (payload.RecipesTouched.Any())
                            {
                                recipesTouched.Add(payload.RecipesTouched[0]);
                                payload.RecipesTouched.RemoveAt(0);
                            }
                            foreach (var deductions in payload.TotalInput)
                            {
                                totalInput.Add(new RecipeFood() { Food = deductions.Food, Amount = deductions.Amount });
                                while (deductions.Amount > 0)
                                {
                                    var fi2 = clonedFoodInventory.FirstOrDefault(foodInstance =>
                                        foodInstance.Food == deductions.Food
                                        && foodInstance.Amount > 0);
                                    if (fi2 is not null)
                                    {
                                        var amountUsed = Math.Min(deductions.Amount, fi2.Amount);
                                        deductions.Amount -= amountUsed;
                                        fi2.Amount -= amountUsed;
                                    }
                                    else
                                    {
                                        deductions.Amount = 0;
                                    }
                                }
                            }

                            foreach (var rawDeductions in payload.RawCost)
                            {
                                rawCost.Add(new RecipeFood() { Food = rawDeductions.Food, Amount = rawDeductions.Amount });
                            }
                            foreach (var additions in payload.TotalOutput)
                            {
                                var fi2 = clonedFoodInventory.FirstOrDefault(foodInstance =>
                                    foodInstance.Food == additions.Food);
                                if (fi2 is not null)
                                {
                                    fi2.Amount += additions.Amount;
                                }
                                else
                                {
                                    var temp = clonedFoodInventory.ToList();
                                    temp.Add(new RecipeFood() { Food = additions.Food, Amount = additions.Amount });
                                    clonedFoodInventory = temp.ToArray();
                                }
                            }
                            continue;
                        }
                    }
                    return new CookPlan() { CanMake = false };
                }
            }
            recipesTouched.Add(recipe);

            return new CookPlan()
            {
                CanMake = true,
                RecipesTouched = recipesTouched,
                TotalInput = totalInput,
                //TotalOutput = new List<RecipeFood>() { recipe.Outputs.First() },
                RawCost = rawCost,
                RecipeName = recipe.Description,
            };
        }

        private static RecipeFood[] GetFoodInstancesFromRecipe(Recipe recipe)
        {
            RecipeFood[] clones = new RecipeFood[recipe.RecipeFoods.Count];
            for (var index = 0; index < recipe.RecipeFoods.Count; index++)
            {
                RecipeFood fi = recipe.RecipeFoods[index];
                clones[index] = (new RecipeFood()
                {
                    Food = fi.Food,
                    Amount = fi.Amount
                });
            }
            return clones;
        }

        private static RecipeFood[] CloneFoodInstances(IList<RecipeFood> foodInstances)
        {
            var clones = new RecipeFood[foodInstances.Count];
            for (var index = 0; index < foodInstances.Count; index++)
            {
                RecipeFood fi = foodInstances[index];
                clones[index] = (new RecipeFood()
                {
                    Food = fi.Food,
                    Amount = fi.Amount
                });
            }
            return clones;
        }
    }
}