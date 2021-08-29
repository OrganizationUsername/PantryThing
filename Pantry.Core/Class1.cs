using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pantry.Core
{
    public static class ExtensionMethods
    {
        public static void ConsoleResult(this GetCookPlan canCook)
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
        public List<BetterRecipe> BetterRecipes { get; set; }
        public static bool operator ==(Food lhs, Food rhs) => rhs is not null && lhs is not null && lhs.FoodId == rhs.FoodId;
        public static bool operator !=(Food lhs, Food rhs) => !(lhs.FoodId == rhs.FoodId);
    }

    public class BetterRecipe
    {
        public List<FoodInstance> Inputs { get; set; }
        public List<FoodInstance> Outputs { get; set; }
        public Food MainOutput { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
    }

    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Description { get; set; }
        public FoodInstance OutputFoodInstance { get; set; }
        public List<FoodInstance> InputFoodInstance { get; set; }
        public double TimeCost { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
        public RecipeHierarchy RecipeHierarchy { get; set; }
    }

    public class RecipeHierarchy
    {
        //maybe each task should have a bunch of children. When traversing,
        //the top parent is usually "plate and serve the food"
        //then the children are all of the last steps required. iterate and
        //schedule all of them, once they are all scheduled, schedule their children
        public List<RecipeHierarchy> Dependents { get; set; }
        public string Instruction { get; set; }
        public double TimeCost { get; set; }
        public List<Equipment> Equipments { get; set; }
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

    public class GetCookPlan
    {
        public string RecipeName;
        public bool CanMake;
        public IList<FoodInstance> TotalOutPut;
        public IList<FoodInstance> TotalCost;
        public IList<FoodInstance> RawCost;
        public List<Recipe> RecipesTouched;
    }
}