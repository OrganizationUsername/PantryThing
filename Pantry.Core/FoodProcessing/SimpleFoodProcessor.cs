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
            CanCookSomething(IList<FoodInstance> foodInventory, Recipe recipe, IList<Recipe> recipes = null)
        {
            List<Recipe> recipesTouched = new();
            var totalInput = new List<FoodInstance>();
            var rawCost = new List<FoodInstance>();
            var recipeLines = GetFoodInstancesFromRecipe(recipe);
            var clonedFoodInventory = CloneFoodInstances(foodInventory);
            foreach (var recipeInputFoodInstance in recipeLines)
            {
                var usedRaw = true;
                while (recipeInputFoodInstance.Amount > 0)
                {
                    var fi = clonedFoodInventory.FirstOrDefault(foodInstance =>
                        foodInstance.FoodType == recipeInputFoodInstance.FoodType
                        && foodInstance.Amount > 0);
                    if (fi is not null /*&& clonedFoodInventory.Where(foodInstance =>
                        foodInstance.FoodType == recipeInputFoodInstance.FoodType).Sum(x => x.Amount) >= recipeInputFoodInstance.Amount*/)
                    {
                        double amountUsed = Math.Min(recipeInputFoodInstance.Amount, fi.Amount);
                        recipeInputFoodInstance.Amount -= amountUsed;
                        fi.Amount -= amountUsed;
                        totalInput.Add(new FoodInstance() { FoodType = fi.FoodType, Amount = amountUsed });
                        if (usedRaw)
                        {
                            rawCost.Add(new FoodInstance() { FoodType = recipeInputFoodInstance.FoodType, Amount = amountUsed });
                        }
                        continue;
                    }
                    usedRaw = false;
                    if (recipes is not null)
                    {
                        var subRecipe = recipes.FirstOrDefault(r =>
                            r.OutputFoodInstance.FoodType == recipeInputFoodInstance.FoodType
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
                                totalInput.Add(new FoodInstance() { FoodType = deductions.FoodType, Amount = deductions.Amount });
                                while (deductions.Amount > 0)
                                {
                                    var fi2 = clonedFoodInventory.FirstOrDefault(foodInstance =>
                                        foodInstance.FoodType == deductions.FoodType
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
                                rawCost.Add(new FoodInstance() { FoodType = rawDeductions.FoodType, Amount = rawDeductions.Amount });
                            }
                            foreach (var additions in payload.TotalOutput)
                            {
                                var fi2 = clonedFoodInventory.FirstOrDefault(foodInstance =>
                                    foodInstance.FoodType == additions.FoodType);
                                if (fi2 is not null)
                                {
                                    fi2.Amount += additions.Amount;
                                }
                                else
                                {
                                    var temp = clonedFoodInventory.ToList();
                                    temp.Add(new FoodInstance() { FoodType = additions.FoodType, Amount = additions.Amount });
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
                TotalOutput = new List<FoodInstance>() { recipe.OutputFoodInstance },
                RawCost = rawCost,
                RecipeName = recipe.Description,
            };
        }

        private static FoodInstance[] GetFoodInstancesFromRecipe(Recipe recipe)
        {
            FoodInstance[] clones = new FoodInstance[recipe.InputFoodInstance.Count];
            for (var index = 0; index < recipe.InputFoodInstance.Count; index++)
            {
                FoodInstance fi = recipe.InputFoodInstance[index];
                clones[index] = (new FoodInstance()
                {
                    FoodType = fi.FoodType,
                    Amount = fi.Amount
                });
            }
            return clones;
        }

        private static FoodInstance[] CloneFoodInstances(IList<FoodInstance> foodInstances)
        {
            var clones = new FoodInstance[foodInstances.Count];
            for (var index = 0; index < foodInstances.Count; index++)
            {
                FoodInstance fi = foodInstances[index];
                clones[index] = (new FoodInstance()
                {
                    FoodType = fi.FoodType,
                    Amount = fi.Amount
                });
            }
            return clones;
        }
    }
}