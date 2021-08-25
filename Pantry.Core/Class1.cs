using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pantry.Core
{
    public static class ExtensionMethods
    {
        public static void ConsoleResult(this CanMakeSomething canCook)
        {
            if (canCook.CanMake)
            {
                Console.WriteLine(string.Join(Environment.NewLine, canCook.TotalCost.Select(x => x.FoodType.Name)));
                Console.WriteLine($"Recipes: {Environment.NewLine}" + string.Join(Environment.NewLine, canCook.RecipesTouched.Select(x => x.Description)));
                //Console.WriteLine($"Time Taken: {canCook.RecipesTouched.Sum(x => x.RecipeSteps.Sum(y => y.TimeCost))}");
                Console.WriteLine($"Total Cost:{Environment.NewLine}"
                                  + string.Join(Environment.NewLine, canCook.RawCost.Select(x => x.FoodType.Name + ": " + x.Amount)));
                Console.WriteLine("-----");
            }
        }

        public static List<FoodInstance> DiminishFoodInstances(this List<FoodInstance> pantry, CanMakeSomething canCook)
        {
            if (canCook.CanMake)
            {
                foreach (var rawCost in canCook.RawCost)
                {
                    while (rawCost.Amount > 0)
                    {
                        var pantryItem =
                            pantry.First(pantry => pantry.FoodType == rawCost.FoodType && pantry.Amount > 0);
                        var AmountToDeduct = Math.Min(pantryItem.Amount, rawCost.Amount);
                        pantryItem.Amount -= AmountToDeduct;
                        rawCost.Amount -= AmountToDeduct;
                    }
                }

                return pantry.Where(pi => pi.Amount > 0).ToList();
            }
            else
            {
                return null;
            }
        }
        public static void OutputRemaining(this List<FoodInstance> pantry)
        {
            if (pantry is null)
            {
                return;
            }
            Console.WriteLine($"Remaining: {Environment.NewLine}" + string.Join(Environment.NewLine, pantry.Where(pi => pi.Amount > 0 && pi.Amount < 10_000_000).Select(pi => pi.FoodType.Name + ": " + pi.Amount)));
        }
    }

    public class Food
    {
        public int FoodId { get; set; }
        public string Name { get; set; }

        public static bool operator ==(Food lhs, Food rhs) => rhs is not null && lhs is not null && lhs.FoodId == rhs.FoodId;

        public static bool operator !=(Food lhs, Food rhs) => !(lhs.FoodId == rhs.FoodId);
    }

    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Description { get; set; }
        public FoodInstance OutputFoodInstance { get; set; }
        public List<FoodInstance> InputFoodInstance { get; set; }
        public double TimeCost { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
    }

    public class RecipeStep
    {
        public int RecipeStepId { get; set; }
        public int RecipeId { get; set; }
        public int Order { get; set; }
        public string Instruction { get; set; }
        public double TimeCost { get; set; }
        public List<Equipment> Equipments { get; set; }
    }

    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; }
        public List<(DateTime startTime, DateTime endTime, string StepName)> BookedTimes { get; set; }
    }

    public class FoodInstance
    {
        public Food FoodType { get; set; }
        public double Amount { get; set; }
        public List<Recipe> Recipes { get; set; } //If I populate all of the FoodInstances in Recipe, I don't have to pass all recipes down to the CanCookSomething method.
        public DateTime Created { get; set; }
    }

    public interface IScheduler
    {
        void TrySchedule(DateTime Goal);
        List<CanMakeSomething> ScheduledTasks { get; set; }
        List<Equipment> Equipments { get; set; }
    }

    public class SimpleScheduler : IScheduler
    {
        //public SimpleScheduler(List<CanMakeSomething> scheduledTasks, List<Equipment> equipments)
        //{
        //    this.ScheduledTasks = scheduledTasks;
        //    this.Equipments = equipments;
        //}

        public List<CanMakeSomething> ScheduledTasks { get; set; } = new List<CanMakeSomething>();
        public List<Equipment> Equipments { get; set; } = new List<Equipment>();
        public void TrySchedule(DateTime goal)
        {
            foreach (var scheduledTask in ScheduledTasks)
            {
                var recipeSteps = scheduledTask.RecipesTouched.SelectMany(b => b.RecipeSteps).Reverse();
                int offset = 0;
                foreach (var recipeStep in recipeSteps)
                {
                    bool satisfied = false;
                    for (; !satisfied; offset++)
                    {
                        if (recipeStep.Equipments.All(y =>
                            y.IsAvailable(goal.AddMinutes(-(offset + recipeStep.TimeCost)), goal.AddMinutes(-offset))))
                        {
                            satisfied = true;
                            foreach (var y in recipeStep.Equipments)
                            {
                                y.BookedTimes.Add(
                                    (goal.AddMinutes(-(offset + recipeStep.TimeCost)),
                                        goal.AddMinutes(-offset), recipeStep.Instruction + $"_ {scheduledTask.RecipeName}")
                                    );
                            }
                        }
                    }
                }
            }

            foreach (var equipment in Equipments)
            {
                Console.WriteLine(equipment.Name);
                foreach (var y in equipment.BookedTimes.OrderBy(z => z.startTime))
                {
                    Console.WriteLine($"{y.startTime.ToShortTimeString()}:{y.endTime.ToShortTimeString()}: {y.StepName}");
                }
            }
        }
    }

    public static class SchedulerExtensions
    {
        public static bool IsAvailable(this Equipment e, DateTime start, DateTime end)
        {
            if (!e.BookedTimes.Any())
            {
                return true;
            }

            foreach (var bookedTime in e.BookedTimes)
            {
                if (false
                    || (bookedTime.startTime < end && bookedTime.endTime >= start))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class CanMakeSomething
    {
        public string RecipeName;
        public bool CanMake;
        public IList<FoodInstance> TotalOutPut;
        public IList<FoodInstance> TotalCost;
        public IList<FoodInstance> RawCost;
        public List<Recipe> RecipesTouched;
    }

    public class CookSomething
    {
        public static CanMakeSomething
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
                            foreach (var deductions in payload.TotalCost)
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
                            foreach (var additions in payload.TotalOutPut)
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
                    return new CanMakeSomething() { CanMake = false };
                }
            }
            recipesTouched.Add(recipe);

            return new CanMakeSomething()
            {
                CanMake = true,
                RecipesTouched = recipesTouched,
                TotalCost = totalInput,
                TotalOutPut = new List<FoodInstance>() { recipe.OutputFoodInstance },
                RawCost = rawCost,
                RecipeName = recipe.Description,
            };
        }

        public static FoodInstance[] GetFoodInstancesFromRecipe(Recipe recipe)
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

        public static FoodInstance[] CloneFoodInstances(IList<FoodInstance> foodInstances)
        {
            FoodInstance[] clones = new FoodInstance[foodInstances.Count];
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