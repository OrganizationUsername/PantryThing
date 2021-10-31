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
        private readonly List<EquipmentType> _equipmentTypes = new();

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
            _equipmentTypes.Add(new EquipmentType() { EquipmentTypeId = 0, EquipmentTypeName = "Sous Vide", Equipments = new List<Equipment>() { _sousVide } });
            _equipmentTypes.Add(new EquipmentType() { EquipmentTypeId = 1, EquipmentTypeName = "Bread Machine", Equipments = new List<Equipment>() { _breadMachine } });
            _equipmentTypes.Add(new EquipmentType() { EquipmentTypeId = 2, EquipmentTypeName = "Human", Equipments = new List<Equipment>() { _humanMachine } });
            _equipmentTypes.Add(new EquipmentType() { EquipmentTypeId = 3, EquipmentTypeName = "Fridge", Equipments = new List<Equipment>() { _fridge } });

            _recipes.Add(
                new()
                {
                    RecipeId = 1,
                    RecipeFoods = new()
                    {
                        new() { Amount = 120, Food = _rawChicken },
                        new() { Amount = 1, Food = _bbqSauce },
                        new() { Amount = -120, Food = _cookedChicken }
                    },
                    RecipeSteps = new()
                    {
                        new()
                        {
                            RecipeStepId = 0,
                            Instruction = "Put chicken in Sous Vide.",
                            TimeCost = 1,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_sousVide, RecipeStepId=0, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Sous Vide")},
                                new(){Equipment = _humanMachine, RecipeStepId=0, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 1,
                            Instruction = "Let it cook.",
                            TimeCost = 120,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_sousVide, RecipeStepId=1, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Sous Vide")},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 2,
                            Instruction = "Take chicken out.",
                            TimeCost = 1,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_sousVide, RecipeStepId=2, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Sous Vide")},
                                new(){Equipment = _humanMachine, RecipeStepId=2, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                    }
                });
            _recipes.Add(
                new()
                {
                    RecipeId = 2,
                    RecipeFoods = new()
                    {
                        new() { Amount = 120, Food = _frozenChicken },
                        new() { Amount = -120, Food = _rawChicken },
                    },
                    RecipeSteps = new()
                    {
                        new()
                        {
                            RecipeStepId = 3,
                            Instruction = "Put chicken in fridge.",
                            TimeCost = 1,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_fridge, RecipeStepId=3, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Fridge")},
                                new(){Equipment = _humanMachine, RecipeStepId=3, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 4,
                            Instruction = "Let it defrost.",
                            TimeCost = 1440,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_fridge, RecipeStepId=4 , EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Fridge")},
                            }
                        }
                    }
                });
            _recipes.Add(
                new()
                {
                    RecipeId = 3,
                    RecipeFoods = new()
                    {
                        new() { Amount = 120, Food = _cookedChicken },
                        new() { Amount = -120, Food = _slicedChicken },
                    },
                    RecipeSteps = new()
                    {
                        new()
                        {
                            RecipeStepId = 5,
                            Instruction = "Cut chicken with a knife.",
                            TimeCost = 3,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment = _humanMachine, RecipeStepId=5, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                    }
                });
            _recipes.Add(
                new()
                {
                    RecipeId = 4,
                    RecipeFoods = new()
                    {
                        new() { Amount = 1, Food = _bread },
                        new() { Amount = -10, Food = _slicedBread },
                    },
                    RecipeSteps = new()
                    {
                        new()
                        {
                            RecipeStepId = 6,
                            Instruction = "Cut Bread",
                            TimeCost = 2,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment = _humanMachine, RecipeStepId=6, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                    }
                });
            _recipes.Add(
                new()
                {
                    RecipeId = 5,
                    RecipeFoods = new()
                    {
                        new() { Amount = 2, Food = _slicedBread },
                        new() { Amount = 120, Food = _slicedChicken },
                        new() { Amount = -1, Food = _chickenSandwich },
                    },
                    RecipeSteps = new()
                    {
                        new()
                        {
                            RecipeStepId = 7,
                            Instruction = "Assemble Sandwich",
                            TimeCost = 1,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment = _humanMachine, RecipeStepId=7, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                    }
                });
            _recipes.Add(
                new()
                {
                    RecipeId = 6,
                    RecipeFoods = new()
                    {
                        new() { Amount = 120, Food = _eggs },
                        new() { Amount = 120, Food = _milk },
                        new() { Amount = 120, Food = _flour },
                        new() { Amount = -1, Food = _bread },
                    },
                    RecipeSteps = new()
                    {
                        new()
                        {
                            RecipeStepId = 8,
                            Instruction = "Insert into Bread Machine.",
                            TimeCost = 1,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_breadMachine, RecipeStepId=8, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Bread Machine")},
                                new(){Equipment = _humanMachine, RecipeStepId=8, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 9,
                            Instruction = "Bread Machine cooks.",
                            TimeCost = 180,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_breadMachine, RecipeStepId=9, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Bread Machine")},
                            }
                        },
                        new()
                        {
                            RecipeStepId = 10,
                            Instruction = "Extract bread from bread machine.",
                            TimeCost = 1,
                            RecipeStepEquipmentType = new List<RecipeStepEquipmentType>()
                            {
                                new(){Equipment =_breadMachine, RecipeStepId=10, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Bread Machine")},
                                new(){ RecipeStepId=10, EquipmentType = _equipmentTypes.First(x=>x.EquipmentTypeName=="Human")},
                            }
                        },
                    }
                });
            _equipments = new() { _breadMachine, _humanMachine, _fridge, _sousVide };
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
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
            CookPlan canCook = null;
            RecipeDag dag = null;
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pp.GetFoodInstances(), recipe, _recipes);
                canCook.ConsoleResult();
                dag ??= canCook.RecipeDag;
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook); Assert.IsTrue(canCook.CanMake);
            //182 + 1 + 2
            var result = AnotherScheduler.GetDagTime(dag);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(result.Value, 185, 0.1);
        }

        [Test]
        public void GetLongestDag()
        {
            List<RecipeFood> pantry = new()
            {
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
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
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
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
                new() { Food = _slicedBread, Amount = 2 },
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
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
                new() { Food = _slicedBread, Amount = 2 },
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
                new() { Food = _rawChicken, Amount = 500 },
                new() { Food = _bbqSauce, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
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
                new() { Food = _slicedBread, Amount = 2 },
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
                new() { Food = _rawChicken, Amount = 500 },
                new() { Food = _bbqSauce, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
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
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
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
                new() { Food = _slicedBread, Amount = 2 },
                new() { Food = _eggs, Amount = 120 },
                new() { Food = _flour, Amount = 210 },
                new() { Food = _milk, Amount = 210 },
                new() { Food = _cookedChicken, Amount = 500 },
                new() { Food = _rawChicken, Amount = 500 },
                new() { Food = _bbqSauce, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            var recipe = _recipes.First(x => x.RecipeFoods.First(y => y.Amount < 0).Food == _chickenSandwich);
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