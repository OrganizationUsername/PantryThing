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

        private readonly List<BetterRecipe> _recipes = new();
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
                new BetterRecipe()
                {
                    RecipeId = 1,
                    MainOutput = _cookedChicken,
                    Inputs = new List<FoodInstance>()
                    {
                        new() {Amount = 120, FoodType = _rawChicken},
                        new() {Amount = 1, FoodType = _bbqSauce}
                    },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _cookedChicken } },
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
                new BetterRecipe()
                {
                    RecipeId = 2,
                    MainOutput = _rawChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _frozenChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _rawChicken } },
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
                new BetterRecipe()
                {
                    RecipeId = 3,
                    MainOutput = _slicedChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _cookedChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _slicedChicken } },
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
                new BetterRecipe()
                {
                    RecipeId = 4,
                    MainOutput = _slicedBread,
                    Inputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _bread } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 10, FoodType = _slicedBread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>() {_humanMachine}
                        },
                    }
                });
            _recipes.Add(
                new BetterRecipe()
                {
                    RecipeId = 5,
                    MainOutput = _chickenSandwich,
                    Inputs = new List<FoodInstance>()
                    {
                        new() {Amount = 2, FoodType = _slicedBread},
                        new() {Amount = 120, FoodType = _slicedChicken}
                    },
                    Outputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _chickenSandwich } },
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
                new BetterRecipe()
                {
                    RecipeId = 6,
                    MainOutput = _bread,
                    Inputs = new List<FoodInstance>()
                    {
                        new() {Amount = 120, FoodType = _eggs},
                        new() {Amount = 120, FoodType = _milk},
                        new() {Amount = 120, FoodType = _flour},
                    },
                    Outputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _bread } },
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
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _eggs, Amount = 120 },
                new FoodInstance() { FoodType = _flour, Amount = 210 },
                new FoodInstance() { FoodType = _milk, Amount = 210 },
                new FoodInstance() { FoodType = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = null;
            RecipeDAG daggy = null;
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                daggy ??= canCook.RecipeDAG;
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
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _eggs, Amount = 120 },
                new FoodInstance() { FoodType = _flour, Amount = 210 },
                new FoodInstance() { FoodType = _milk, Amount = 210 },
                new FoodInstance() { FoodType = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = null;
            List<RecipeDAG> dags = new();
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDAG);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var result = AnotherScheduler.GetLongestDag(dags);
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
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _eggs, Amount = 120 },
                new FoodInstance() { FoodType = _flour, Amount = 210 },
                new FoodInstance() { FoodType = _milk, Amount = 210 },
                new FoodInstance() { FoodType = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = null;
            List<RecipeDAG> dags = new();
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDAG);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var result = AnotherScheduler.GetLongestDag(dags);
            Console.WriteLine("-----");
            Console.WriteLine("Longest");
            Assert.AreEqual(ExtensionMethods.GetDagString(result), "Chicken Sandwich->Sliced Bread->Bread");
        }

        [Test]
        public void CheckLastDagText()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _eggs, Amount = 120 },
                new FoodInstance() { FoodType = _flour, Amount = 210 },
                new FoodInstance() { FoodType = _milk, Amount = 210 },
                new FoodInstance() { FoodType = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = null;
            List<RecipeDAG> dags = new();
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                dags.Add(canCook.RecipeDAG);
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            var result = AnotherScheduler.GetLongestDag(dags);
            Console.WriteLine("-----");
            Console.WriteLine("Last");
            Console.WriteLine(ExtensionMethods.GetDagString(dags.Last()));
            Assert.AreEqual(ExtensionMethods.GetDagString(dags.Last()), "Chicken Sandwich->Sliced Chicken");
        }
    }

    public class AnotherScheduler
    {
        public List<RecipeDAG> Dags { get; set; }
        public List<Equipment> Equipments { get; set; }

        public AnotherScheduler(List<RecipeDAG> dags, List<Equipment> equipments)
        {
            Dags = dags;
            Equipments = equipments;
        }

        public static double? GetDagTime(RecipeDAG dag)
        {
            //I guess the easiest way is to go down from every base to tip and add stuff.
            //There's probably a clever Max thing that can be done.
            //What if I could store in a .ToLookup with (double Time, Dag)
            //lol, whatever
            if (dag is null) { return null; }
            var thisGuysCost = dag.MainRecipe.RecipeSteps.Sum(x => x.TimeCost);
            if (dag.SubordinateBetterRecipes is null || dag.SubordinateBetterRecipes.Count == 0)
            {
                return thisGuysCost;
            }
            return thisGuysCost + dag.SubordinateBetterRecipes.Max(GetDagTime);
        }

        public static RecipeDAG GetLongestDag(List<RecipeDAG> dags)
        {
            var y = dags.ToLookup(x => GetDagTime(x), x => x);
            return dags.OrderBy(x => GetDagTime(x) ?? -1).Last();
        }

        public void TrySchedule()
        {
            var x = Dags.Count + Equipments.Count;
            Console.WriteLine(x);
        }

    }


}