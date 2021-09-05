using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pantry.Core.Extensions;
using Pantry.Core.FoodProcessing;
using Pantry.Core.Models;
using Pantry.Core.Scheduler;

namespace Pantry.Core.Test.ScheduleTests
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
        private readonly IFoodProcessor _foodProcessor = new SimpleFoodProcessor();

        [SetUp]
        public void Setup()
        {
            Recipes = new List<Recipe>
            {
                new Recipe()
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
                    new(){Order = 1,
                        Instruction = "Assemble Sandwich.",
                        TimeCost = 3,
                        Equipments = new List<Equipment>(){_humanMachine}},
                },
                    RecipeHierarchy = new RecipeHierarchy()
                    {
                        Instruction = "Assemble Sandwich",
                        TimeCost = 3,
                        Equipments = new List<Equipment>(){_humanMachine},
                        Dependents = new List<RecipeHierarchy>() {}
                    },
                },
                new Recipe()
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
                    RecipeHierarchy = new RecipeHierarchy()
                    {
                        Instruction = "Do its thing.",
                        TimeCost = 40,
                        Equipments = new List<Equipment>(){_breadMachine},
                        Dependents = new List<RecipeHierarchy>()
                        {
                             new RecipeHierarchy()
                            {
                                Instruction = "Fill/operate bread machine.",
                                TimeCost = 1,
                                Equipments = new List<Equipment>(){_breadMachine, _humanMachine},
                                Dependents = new List<RecipeHierarchy>() { }
                            }
                        }
                    },
                },
                new Recipe()
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
                    RecipeHierarchy = new RecipeHierarchy()
                    {
                        Instruction = "Do its thing.",
                        TimeCost = 40,
                        Equipments = new List<Equipment>(){_riceMachine},
                        Dependents = new List<RecipeHierarchy>()
                        {
                            new RecipeHierarchy()
                            {
                                Instruction = "Fill/operate rice machine.",
                                TimeCost = 1,
                                Equipments = new List<Equipment>(){ _riceMachine, _humanMachine},
                                Dependents = new List<RecipeHierarchy>() { }
                            }
                        }
                    },
                },
                new Recipe()
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
                    RecipeHierarchy = new RecipeHierarchy()
                    {
                        Instruction = "Heat and plate.",
                        TimeCost = 5,
                        Equipments = new List<Equipment>(){_stoveTop, _humanMachine},
                        Dependents = new List<RecipeHierarchy>()
                    },
                }
            };
            _equipments = new List<Equipment>() { _breadMachine, _humanMachine, _riceMachine, _stoveTop };
        }

        [Test]
        public void NaiveScheduleMealsBeforeATime()
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
            var pp = new PantryProvider(pantry);
            var cms = new List<CookPlan>();
            Recipe recipe = default;
            CookPlan canCook = default;
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            cms.Add(canCook);
            canCook.ConsoleResult();
            pantry = pp.AdjustOnHandQuantity(canCook);
            Assert.IsTrue(canCook.CanMake);
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _curryMeal);
            canCook = _foodProcessor.CanCookSomething(pp.GetFoodInstances(), recipe, Recipes);
            cms.Add(canCook);
            canCook.ConsoleResult();
            pantry = pp.AdjustOnHandQuantity(canCook);
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsTrue(canCook.CanMake);
            Console.WriteLine("-----");

            foreach (var canMakeSomething in cms)
            {
                Console.WriteLine(string.Join(Environment.NewLine, canMakeSomething.RecipesTouched.Select(y => y.Description + ": " + y.RecipeSteps.Sum(z => z.TimeCost))));
                Console.WriteLine("-----");
            }
            IScheduler simpleScheduler = new NaiveScheduler(cms, _equipments);
            simpleScheduler.TrySchedule(DateTime.Parse("2021/08/15 18:00"));
        }

        [Test]
        public void SimpleScheduleMealsBeforeATime()
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
            var pp = new PantryProvider(pantry);
            var cms = new List<CookPlan>();
            Recipe recipe = default;
            CookPlan canCook = default;
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            cms.Add(canCook);
            canCook.ConsoleResult();
            pantry = pp.AdjustOnHandQuantity(canCook);
            Assert.IsTrue(canCook.CanMake);
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _curryMeal);
            canCook = _foodProcessor.CanCookSomething(pp.GetFoodInstances(), recipe, Recipes);
            cms.Add(canCook);
            canCook.ConsoleResult();
            pantry = pp.AdjustOnHandQuantity(canCook);
            pp.GetFoodInstances().OutputRemaining();
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

  
}