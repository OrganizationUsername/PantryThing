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
        { EquipmentName = "Bread Machine", EquipmentCommitments = new List<EquipmentCommitment>() };

        private readonly Equipment _humanMachine = new()
        { EquipmentName = "Human", EquipmentCommitments = new List<EquipmentCommitment>() };

        private readonly Equipment _fridge = new()
        { EquipmentName = "Fridge", EquipmentCommitments = new List<EquipmentCommitment>() };

        private readonly Equipment _sousVide = new()
        { EquipmentName = "Sous Vide", EquipmentCommitments = new List<EquipmentCommitment>() };
        private List<Equipment> _equipments;

        private readonly List<Recipe> _recipes = new();
        private readonly BetterFoodProcessor _foodProcessor = new();
        private readonly Food _frozenChicken = new() { FoodId = 0, FoodName = "Frozen Chicken" };
        private readonly Food _rawChicken = new() { FoodId = 1, FoodName = "Raw Chicken", };
        private readonly Food _bbqSauce = new() { FoodId = 2, FoodName = "BBQ Sauce" };
        private readonly Food _cookedChicken = new() { FoodId = 3, FoodName = "Cooked Chicken" };
        private readonly Food _slicedChicken = new() { FoodId = 4, FoodName = "Sliced Chicken" };
        private readonly Food _flour = new() { FoodId = 5, FoodName = "Flour" };
        private readonly Food _eggs = new() { FoodId = 6, FoodName = "Eggs" };
        private readonly Food _milk = new() { FoodId = 7, FoodName = "Milk" };
        private readonly Food _bread = new() { FoodId = 8, FoodName = "Bread" };
        private readonly Food _slicedBread = new() { FoodId = 9, FoodName = "Sliced Bread" };
        private readonly Food _chickenSandwich = new() { FoodId = 10, FoodName = "Chicken Sandwich" };

        [SetUp]
        public void Setup()
        {
            _recipes.Add(
                new Recipe()
                {
                    RecipeId = 1,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() {Amount = 120, Food = _rawChicken},
                        new() {Amount = 1, Food = _bbqSauce},
                        new() { Amount = -120, Food = _cookedChicken }
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            RecipeStepId = 0,
                            Instruction = "Put chicken in Sous Vide.", TimeCost = 1,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_sousVide, RecipeStepId=0 },
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=0},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 1,
                            Instruction = "Let it cook.", TimeCost = 120,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_sousVide, RecipeStepId=1 },
                            }
                        },
                        new()
                        {
                            RecipeStepId = 2,
                            Instruction = "Take chicken out.", TimeCost = 1,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_sousVide, RecipeStepId=2 },
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=2},
                            }
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    RecipeId = 2,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() { Amount = 120, Food = _frozenChicken },
                        new() { Amount = -120, Food = _rawChicken },
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            RecipeStepId = 3,
                            Instruction = "Put chicken in fridge.", TimeCost = 1,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_fridge, RecipeStepId=3 },
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=3},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 4,
                            Instruction = "Let it defrost.", TimeCost = 1440,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_fridge, RecipeStepId=4 },
                            }
                        }
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    RecipeId = 3,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() { Amount = 120, Food = _cookedChicken },
                        new() { Amount = -120, Food = _slicedChicken },
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            RecipeStepId = 5,
                            Instruction = "Cut chicken with a knife.", TimeCost = 3,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=5},
                            }
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    RecipeId = 4,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() { Amount = 1, Food = _bread },
                        new() { Amount = -10, Food = _slicedBread },
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            RecipeStepId = 6,
                            Instruction = "Cut Bread", TimeCost = 2,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=6},
                            }
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    RecipeId = 5,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() {Amount = 2, Food = _slicedBread},
                        new() {Amount = 120, Food = _slicedChicken},
                        new() { Amount = -1, Food = _chickenSandwich },
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            RecipeStepId = 7,
                            Instruction = "Assemble Sandwich", TimeCost = 1,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=7},
                            }
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    RecipeId = 6,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() {Amount = 120, Food = _eggs},
                        new() {Amount = 120, Food = _milk},
                        new() {Amount = 120, Food = _flour},
                        new() { Amount = -1, Food = _bread },
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            RecipeStepId = 8,
                            Instruction = "Insert into Bread Machine.", TimeCost = 1,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_breadMachine, RecipeStepId=8 },
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=8},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 9,
                            Instruction = "Bread Machine cooks.", TimeCost = 180,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_breadMachine, RecipeStepId=9 },
                            }
                        },
                        new()
                        {
                            RecipeStepId = 10,
                            Instruction = "Extract bread from bread machine.", TimeCost = 1,
                            RecipeStepEquipment = new List<RecipeStepEquipment>()
                            {
                                new RecipeStepEquipment(){Equipment =_breadMachine, RecipeStepId=10 },
                                new RecipeStepEquipment(){Equipment = _humanMachine, RecipeStepId=10},
                            }
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
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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
            var recipe = _recipes.First(x => x.RecipeFoods.First(x => x.Amount < 0).Food == _chickenSandwich);
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