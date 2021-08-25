using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pantry.Core.Scheduler;

namespace Pantry.Core.Test
{
    public class ScheduleTests
    {
        public List<Recipe> Recipes;
        private readonly Food _water = new() { FoodId = 0, Name = "Water" };
        private readonly Food _bread = new() { FoodId = 1, Name = "BreadSlice" };
        private readonly Food _cheese = new() { FoodId = 2, Name = "CheeseSlice" };
        private readonly Food _flour = new() { FoodId = 3, Name = "Flour" };
        private readonly Food _milk = new() { FoodId = 4, Name = "Milk" };
        private readonly Food _cheeseSandwich = new() { FoodId = 5, Name = "CheeseSandwich" };
        private readonly Food _curryMeal = new() { FoodId = 8, Name = "CurryMeal" };
        private readonly Food _cookedRice = new() { FoodId = 9, Name = "CookedRice" };
        private readonly Food _currySauce = new() { FoodId = 10, Name = "CurrySauce" };
        private readonly Food _rice = new() { FoodId = 11, Name = "Rice" };
        private readonly Equipment _breadMachine = new() { Name = "Bread Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _riceMachine = new() { Name = "Rice Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _humanMachine = new() { Name = "Human", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _stoveTop = new() { Name = "StoveTop", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private List<Equipment> _equipments;
        private FoodProcessor FoodProcessor = new FoodProcessor();

        [SetUp]
        public void Setup()
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            Recipes = new List<Recipe>();
#pragma warning restore IDE0028 // Simplify collection initialization
            Recipes.Add(new Recipe()
            {
                RecipeId = 1,
                Description = "Cheese sandwich",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _bread, Amount = 2 },
                    new() { FoodType = _cheese, Amount = 1 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _cheeseSandwich, Amount = 1 },
                TimeCost = 3,
                RecipeSteps = new List<RecipeStep>()
                {
                    new(){Order = 1,Instruction = "Assemble Sandwich.", TimeCost = 3, Equipments = new List<Equipment>(){_humanMachine}},
                },
            });
            Recipes.Add(new Recipe()
            {
                RecipeId = 2,
                Description = "Bread",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _flour, Amount = 2 },
                    new() { FoodType = _milk, Amount = 1 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _bread, Amount = 10 },
                TimeCost = 45,
                RecipeSteps = new List<RecipeStep>()
                {
                    new(){Order = 1,Instruction = "Fill/operate bread machine.", TimeCost = 1, Equipments = new List<Equipment>(){_breadMachine, _humanMachine}},
                    new(){Order = 2,Instruction = "Do its thing.", TimeCost = 40, Equipments = new List<Equipment>(){_breadMachine}},
                },
            });
            Recipes.Add(new Recipe()
            {
                RecipeId = 10,
                Description = "CookedRice",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _rice, Amount = 1 },
                    new() { FoodType = _water, Amount = 3 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _cookedRice, Amount = 4 },
                TimeCost = 60,
                RecipeSteps = new List<RecipeStep>()
                {
                    new(){Order = 1,Instruction = "Fill/operate rice machine.", TimeCost = 1, Equipments = new List<Equipment>(){_riceMachine, _humanMachine}},
                    new(){Order = 2,Instruction = "Do its thing.", TimeCost = 40, Equipments = new List<Equipment>() {_riceMachine}},
                },
            });
            Recipes.Add(new Recipe()
            {
                RecipeId = 11,
                Description = "CurriedRice",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _cookedRice, Amount = 4 },
                    new() { FoodType = _currySauce, Amount = 2 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _curryMeal, Amount = 6 },
                TimeCost = 5,
                RecipeSteps = new List<RecipeStep>()
                {
                    new(){Order = 1,Instruction = "Heat and plate.", TimeCost = 5, Equipments = new List<Equipment>() {_stoveTop, _humanMachine}},
                },
            });
            _equipments = new List<Equipment>() { _breadMachine, _humanMachine, _riceMachine, _stoveTop };
        }

        [Test]
        public void ScheduleMealsBeforeATime()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 4 },
                new FoodInstance() { FoodType = _milk, Amount = 3 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
                new FoodInstance() { FoodType = _bread, Amount = 1 },
                new FoodInstance() { FoodType = _water, Amount = double.MaxValue },
                new FoodInstance() { FoodType = _rice, Amount = 100 },
                new FoodInstance() { FoodType = _currySauce, Amount = 10 },
            };
            var cms = new List<GetCookPlan>();
            Recipe recipe = default;
            GetCookPlan canCook = default;
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            canCook = FoodProcessor.CanCookSomething(pantry, recipe, Recipes);
            cms.Add(canCook);
            canCook.ConsoleResult();
            pantry = pantry.DiminishFoodInstances(canCook);
            Assert.IsTrue(canCook.CanMake);
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _curryMeal);
            canCook = FoodProcessor.CanCookSomething(pantry, recipe, Recipes);
            cms.Add(canCook);
            canCook.ConsoleResult();
            pantry = pantry.DiminishFoodInstances(canCook);
            pantry.OutputRemaining();
            Assert.IsTrue(canCook.CanMake);
            Console.WriteLine("-----");

            foreach (var canMakeSomething in cms)
            {
                Console.WriteLine(string.Join(Environment.NewLine, canMakeSomething.RecipesTouched.Select(y => y.Description + ": " + y.RecipeSteps.Sum(z => z.TimeCost))));
                Console.WriteLine("-----");
            }
            IScheduler simpleScheduler = new SimpleScheduler(cms, _equipments);
            simpleScheduler.TrySchedule(DateTime.Parse("2021/08/15 18:00"));
        }
    }




    /// <summary>
    /// Provides the pantry at a certain time and does everything else.
    /// This object should also be able to watch you go into the negatives.
    /// Like if your plan has you using bread on Friday, but you want to use it all up on Thursday but start a new loaf, it could use
    /// the deficit to make a plan to make more.
    /// For this reason there will be some hypothetical RecipeThingys.
    /// </summary>
    public class PantryController
    {
        /// <summary>
        /// One of the outputs it might give is a pantry at an instance, so a DateTime might be nice
        /// </summary>
        public DateTime DateTime { get; set; }
        public List<FoodInstance> fis { get; set; }
        public IRecipeThingy RealRecipeThingy { get; set; }
        public IScheduler Scheduler { get; set; }
        public IPantryProvider PantryProvider { get; set; }
        public IFoodProcessor FoodProcessor { get; set; }

        public void UseFood(GetCookPlan canCook)
        {
            fis = RealRecipeThingy.UseFood(canCook, fis);
        }
    }

    public interface IPantryProvider
    {
        List<FoodInstance> GetFoodInstances();
    }

    public class PantryProvider : IPantryProvider
    {
        public List<FoodInstance> GetFoodInstances()
        {
            return null;
        }
    }

    public interface IRecipeThingy
    {
        List<FoodInstance> UseFood(GetCookPlan canCook, List<FoodInstance> fis);
    }

    /// <summary>
    /// This RecipeProcessor doesn't allow you to go into the negatives, and also requires that you can cook it.
    /// </summary>
    public class RealRecipeThingy : IRecipeThingy
    {
        public List<FoodInstance> UseFood(GetCookPlan canCook, List<FoodInstance> fis)
        {
            if (!canCook.CanMake)
            {
                return null;
            }
            foreach (var rawCost in canCook.RawCost)
            {
                while (rawCost.Amount > 0)
                {
                    var pantryItem =
                        fis.First(pantry => pantry.FoodType == rawCost.FoodType && pantry.Amount > 0);
                    var amountToDeduct = Math.Min(pantryItem.Amount, rawCost.Amount);
                    pantryItem.Amount -= amountToDeduct;
                    rawCost.Amount -= amountToDeduct;
                }
            }
            return fis.Where(pi => pi.Amount > 0).ToList();
        }
    }

}