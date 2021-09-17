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
    public class NewScheduleTests
    {
        private readonly Equipment _breadMachine = new()
        { Name = "Bread Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private readonly Equipment _humanMachine = new()
        { Name = "Human", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private readonly Equipment _fridge = new()
        { Name = "Fridge", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private readonly Equipment _sousVide = new()
        { Name = "Sous Vide", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private List<Equipment> _equipments;

        private readonly List<Recipe> _recipes = new();
        private readonly BetterFoodProcessor _foodProcessor = new();
        private readonly Food _frozenChicken = new() { FoodId = 0, Name = "Frozen Chicken", BetterRecipes = null };
        private readonly Food _rawChicken = new() { FoodId = 1, Name = "Raw Chicken", };
        private readonly Food _bbqSauce = new() { FoodId = 2, Name = "BBQ Sauce", BetterRecipes = null };
        private readonly Food _cookedChicken = new() { FoodId = 3, Name = "Cooked Chicken" };
        private readonly Food _slicedChicken = new() { FoodId = 4, Name = "Sliced Chicken" };
        private readonly Food _flour = new() { FoodId = 5, Name = "Flour", BetterRecipes = null };
        private readonly Food _eggs = new() { FoodId = 6, Name = "Eggs", BetterRecipes = null };
        private readonly Food _milk = new() { FoodId = 7, Name = "Milk", BetterRecipes = null };
        private readonly Food _bread = new() { FoodId = 8, Name = "Bread" };
        private readonly Food _slicedBread = new() { FoodId = 9, Name = "Sliced Bread" };
        private readonly Food _chickenSandwich = new() { FoodId = 10, Name = "Chicken Sandwich" };

        [SetUp]
        public void Setup()
        {
            _recipes.Add(
                new Recipe()
                {
                    Id = 1,
                    Inputs = new List<RecipeFood>()
                    {
                        new() {Amount = 120, Food = _rawChicken},
                        new() {Amount = 1, Food = _bbqSauce},
                        new() { Amount = -120, Food = _cookedChicken }
                    },
                    Outputs = new List<RecipeFood>() { new() { Amount = 120, Food = _cookedChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Put chicken in Sous Vide.", TimeCost = 1,
                            Equipments = new List<Equipment>() {_sousVide, _humanMachine}
                        },
                        new()
                        {
                            Instruction = "Let it cook.", TimeCost = 120, Equipments = new List<Equipment>() {_sousVide}
                        },
                        new()
                        {
                            Instruction = "Take chicken out.", TimeCost = 1,
                            Equipments = new List<Equipment>() {_sousVide, _humanMachine}
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 2,
                    Inputs = new List<RecipeFood>()
                    {
                        new() { Amount = 120, Food = _frozenChicken },
                        new() { Amount = -120, Food = _rawChicken },
                    },
                    Outputs = new List<RecipeFood>() { new() { Amount = 120, Food = _rawChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Put chicken in fridge.", TimeCost = 1,
                            Equipments = new List<Equipment>() {_fridge, _humanMachine}
                        },
                        new()
                        {
                            Instruction = "Let it defrost.", TimeCost = 1440,
                            Equipments = new List<Equipment>() {_fridge}
                        }
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 3,
                    Inputs = new List<RecipeFood>()
                    {
                        new() { Amount = 120, Food = _cookedChicken },
                        new() { Amount = -120, Food = _slicedChicken },
                    },
                    Outputs = new List<RecipeFood>() { new() { Amount = 120, Food = _slicedChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Cut chicken with a knife.", TimeCost = 3,
                            Equipments = new List<Equipment>() {_humanMachine}
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 4,
                    Inputs = new List<RecipeFood>()
                    {
                        new() { Amount = 1, Food = _bread },
                        new() { Amount = -10, Food = _slicedBread },
                    },
                    Outputs = new List<RecipeFood>() { new() { Amount = 10, Food = _slicedBread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>() {_humanMachine}
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 5,
                    Inputs = new List<RecipeFood>()
                    {
                        new() {Amount = 2, Food = _slicedBread},
                        new() {Amount = 120, Food = _slicedChicken},
                        new() { Amount = -1, Food = _chickenSandwich },
                    },
                    Outputs = new List<RecipeFood>() { new() { Amount = 1, Food = _chickenSandwich } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Assemble Sandwich", TimeCost = 1,
                            Equipments = new List<Equipment>() {_humanMachine}
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 6,
                    Inputs = new List<RecipeFood>()
                    {
                        new() {Amount = 120, Food = _eggs},
                        new() {Amount = 120, Food = _milk},
                        new() {Amount = 120, Food = _flour},
                        new() { Amount = -1, Food = _bread },
                    },
                    Outputs = new List<RecipeFood>() { new() { Amount = 1, Food = _bread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Insert into Bread Machine.", TimeCost = 1,
                            Equipments = new() {_humanMachine, _breadMachine}
                        },
                        new()
                        {
                            Instruction = "Bread Machine cooks.", TimeCost = 180, Equipments = new() {_breadMachine}
                        },
                        new()
                        {
                            Instruction = "Extract bread from bread machine.", TimeCost = 1,
                            Equipments = new() {_humanMachine, _breadMachine}
                        },
                    }
                });
            _equipments = new List<Equipment>() { _breadMachine, _humanMachine, _fridge, _sousVide };
        }

        [Test]
        public void GetNullPathTime()
        {
            var result = AnotherScheduler.GetDagTime(null);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void GetDagLength()
        {
            //This is way too long. At some point I have to create some abstractions to cut this down.

            //One heuristic is to get the longest path and schedule that first.
            //If we got the length of the
            //Chicken Sandwich->Sliced Bread->Bread
            //path, then I would know it's the correct one to schedule first to 

            //I could create an object that holds all of the jobs, maybe even assigns them GUIDs.
            //If I were cooking more than one thing, I could also have the opportunity of checking
            //whether or not pushing back a long-running process allows to fit in something else speeds
            //up overall flow.
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x => x.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = null;
            RecipeDag daggy = null;
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                daggy ??= canCook.RecipeDag;
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            //182 + 1 + 2
            var result = AnotherScheduler.GetDagTime(daggy);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(result.Value, 185, 0.1);
        }

        [Test]
        public void GetLongestDag()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x => x.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = null;
            List<RecipeDag> dags = new();
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDag);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var result = AnotherScheduler.GetLongestUnresolvedDag(dags);
            Console.WriteLine("-----");
            Console.WriteLine("Longest");
            Console.WriteLine(ExtensionMethods.GetDagString(result));
            var resultTime = AnotherScheduler.GetDagTime(result);
            Assert.IsTrue(resultTime.HasValue);
            Assert.AreEqual(resultTime.Value, 185, 0.1);
        }

        [Test]
        public void GetLongestDagText()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x => x.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = null;
            List<RecipeDag> dags = new();
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDag);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var allDags = dags.SelectMany(AnotherScheduler.DecomposeDags).ToList();
            var result = AnotherScheduler.GetLongestUnresolvedDag(allDags);
            Console.WriteLine("-----");
            Console.WriteLine("Longest");
            Console.WriteLine(ExtensionMethods.GetDagString(result));
            Console.WriteLine("-----");
            Console.WriteLine("All");
            Console.WriteLine(string.Join(Environment.NewLine, allDags.Select(x => ExtensionMethods.GetDagString(x) + $"- {AnotherScheduler.GetDagTime(x)}")));
            Assert.AreEqual(ExtensionMethods.GetDagString(result), "Chicken Sandwich->Sliced Bread->Bread");
        }

        [Test]
        public void GetLongestDagTextWithLimitedBread()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 2 },
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x => x.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = default;
            var dags = new List<RecipeDag>();
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDag);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var allDags = dags.SelectMany(AnotherScheduler.DecomposeDags).ToList();
            var result = AnotherScheduler.GetLongestUnresolvedDag(allDags);
            Console.WriteLine("-----");
            Console.WriteLine("Longest");
            Console.WriteLine(ExtensionMethods.GetDagString(result));
            Console.WriteLine("-----");
            Console.WriteLine("All");
            Console.WriteLine(string.Join(Environment.NewLine, allDags.Select(x => ExtensionMethods.GetDagString(x) + $"- {AnotherScheduler.GetDagTime(x)}")));
            Assert.AreEqual(ExtensionMethods.GetDagString(result), "Chicken Sandwich->Sliced Bread->Bread");
        }

        [Test]
        public void GetSecondLongestDagText()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 2 },
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
                new RecipeFood() { Food = _rawChicken, Amount = 500 },
                new RecipeFood() { Food = _bbqSauce, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x => x.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = default;
            var dags = new List<RecipeDag>();
            for (var i = 0; i < 5; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDag);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var allDags = dags.SelectMany(AnotherScheduler.DecomposeDags).ToList();
            var tempResult = AnotherScheduler.GetLongestUnresolvedDag(allDags);
            tempResult.Scheduled = true;
            var result = AnotherScheduler.GetLongestUnresolvedDag(allDags);
            Console.WriteLine("-----");
            Console.WriteLine("Longest");
            Console.WriteLine(ExtensionMethods.GetDagString(tempResult));
            Console.WriteLine("-----");
            Console.WriteLine("2nd Longest");
            Console.WriteLine(ExtensionMethods.GetDagString(result));
            Console.WriteLine("-----");
            Console.WriteLine("All");
            Console.WriteLine(string.Join(Environment.NewLine, allDags.Select(x => ExtensionMethods.GetDagString(x) + $"- {AnotherScheduler.GetDagTime(x)}")));
            Assert.AreEqual(ExtensionMethods.GetDagString(result), "Chicken Sandwich->Sliced Chicken->Cooked Chicken");
        }

        [Test]
        public void GetSecondLongestDagText_Bad()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 2 },
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
                new RecipeFood() { Food = _rawChicken, Amount = 500 },
                new RecipeFood() { Food = _bbqSauce, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x => x.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = default;
            var dags = new List<RecipeDag>();
            for (var i = 0; i < 5; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDag);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var allDags = dags.SelectMany(AnotherScheduler.DecomposeDags).ToList();
            var tempResult = AnotherScheduler.GetLongestUnresolvedDag(allDags);
            //tempResult.Scheduled = true; //It hasn't been scheduled, so both tempResult/result will be the the longest
            var result = AnotherScheduler.GetLongestUnresolvedDag(allDags);
            Console.WriteLine("-----");
            Console.WriteLine("Longest");
            Console.WriteLine(ExtensionMethods.GetDagString(tempResult));
            Console.WriteLine("-----");
            Console.WriteLine("2nd Longest");
            Console.WriteLine(ExtensionMethods.GetDagString(result));
            Console.WriteLine("-----");
            Console.WriteLine("All");
            Console.WriteLine(string.Join(Environment.NewLine, allDags.Select(x => ExtensionMethods.GetDagString(x) + $"- {AnotherScheduler.GetDagTime(x)}")));
            Assert.AreNotEqual(ExtensionMethods.GetDagString(result), "Chicken Sandwich->Sliced Chicken->Cooked Chicken");
            Assert.AreEqual(ExtensionMethods.GetDagString(result), "Chicken Sandwich->Sliced Bread->Bread");
            Assert.AreEqual(ExtensionMethods.GetDagString(tempResult), "Chicken Sandwich->Sliced Bread->Bread");
        }

        [Test]
        public void CheckLastDagText()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x => x.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = null;
            List<RecipeDag> dags = new();
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDag);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var result = AnotherScheduler.GetLongestUnresolvedDag(dags);
            Console.WriteLine("-----");
            Console.WriteLine("Last");
            Console.WriteLine(ExtensionMethods.GetDagString(dags.Last()));
            Assert.AreEqual(ExtensionMethods.GetDagString(dags.Last()), "Chicken Sandwich->Sliced Chicken");
        }

        [Test]
        public void AnActualSchedulingTest()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 2 },
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
                new RecipeFood() { Food = _rawChicken, Amount = 500 },
                new RecipeFood() { Food = _bbqSauce, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.Inputs.First(x=>x.Amount<0).Food == _chickenSandwich);
            CookPlan canCook = default;
            var dags = new List<RecipeDag>();
            for (var i = 0; i < 5; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDag);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            Console.WriteLine($"{Environment.NewLine}All Dag text in original order:" + string.Join(Environment.NewLine, dags.Select(x => ExtensionMethods.GetDagString(x) + $"- {AnotherScheduler.GetDagTime(x)}")));
            var another = new AnotherScheduler(DateTime.Parse("2021/08/15 18:00"), dags, _equipments);
            another.TrySchedule();
        }

    }
}