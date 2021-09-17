using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pantry.Core.Extensions;
using Pantry.Core.FoodProcessing;
using Pantry.Core.Models;

namespace Pantry.Core.Test.Recipe_Tests
{

    public class NewRecipeTests
    {
        private readonly Equipment _breadMachine = new()
        { Name = "Bread Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _humanMachine = new()
        { Name = "Human", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _fridge = new()
        { Name = "Fridge", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _sousVide = new()
        { Name = "Sous Vide", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

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
                    //MainOutput = _cookedChicken,
                    RecipeFoods = new List<RecipeFood>() {
                        new(){Amount = 0120, Food = _rawChicken },
                        new(){Amount = 0001, Food = _bbqSauce },
                        new(){Amount = -120, Food = _cookedChicken }
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Put chicken in Sous Vide.", TimeCost = 1, Equipments = new List<Equipment>() {_sousVide, _humanMachine}},
                        new() {Instruction = "Let it cook.", TimeCost = 120, Equipments = new List<Equipment>() { _sousVide }},
                        new() {Instruction = "Take chicken out.", TimeCost = 1, Equipments = new List<Equipment>() {_sousVide, _humanMachine}},
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 2,
                    //MainOutput = _rawChicken,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() { Amount = 120, Food = _frozenChicken } ,
                        new() { Amount = -120, Food = _rawChicken }
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Put chicken in fridge.", TimeCost = 1,Equipments = new List<Equipment>() {_fridge, _humanMachine}},
                        new() {Instruction = "Let it defrost.", TimeCost = 1440, Equipments = new List<Equipment>() {_fridge}}
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 3,
                    MainOutput = _slicedChicken,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() { Amount = 120, Food = _cookedChicken },
                        new() { Amount = -120, Food = _slicedChicken }
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Cut chicken with a knife.", TimeCost = 3, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 4,
                    RecipeFoods = new List<RecipeFood>()
                    {
                        new() { Amount = 1, Food = _bread },
                        new() { Amount = -10, Food = _slicedBread }
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 5,
                    RecipeFoods = new List<RecipeFood>() {
                        new() { Amount = 2, Food = _slicedBread } ,
                        new() { Amount = 120, Food = _slicedChicken} ,
                        new() { Amount = -1, Food = _chickenSandwich }
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Assemble Sandwich", TimeCost = 1, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 6,
                    RecipeFoods = new List<RecipeFood>() {
                        new() { Amount = 120, Food = _eggs } ,
                        new() { Amount = 120, Food = _milk} ,
                        new() { Amount = 120, Food = _flour } ,
                        new() { Amount = -1, Food = _bread },
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Insert into Bread Machine.", TimeCost = 1, Equipments = new(){_humanMachine, _breadMachine}},
                        new() {Instruction = "Bread Machine cooks.", TimeCost = 180, Equipments = new(){ _breadMachine}},
                        new() {Instruction = "Extract bread from bread machine.", TimeCost = 1, Equipments = new(){_humanMachine, _breadMachine}},
                    }
                });
        }

        [Test]
        public void BetterFoodProcessorSimple_OK()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 10 },
                new RecipeFood() { Food = _slicedChicken, Amount = 120 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorSimple_Fail()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 10 },
                new RecipeFood() { Food = _slicedChicken, Amount = 119 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorSimple_InsufficientBread_Fail()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 1 },
                new RecipeFood() { Food = _slicedChicken, Amount = 120 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorSimpleNested_OK()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 10 },
                new RecipeFood() { Food = _cookedChicken, Amount = 120 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorComplexNested_OK()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _bread, Amount = 10 },
                new RecipeFood() { Food = _cookedChicken, Amount = 120 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich && y.Amount < 0)); ;
            //recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorNested_Fail()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 210 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 120 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorWorstCaseNested()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 210 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _frozenChicken, Amount = 120 },
                new RecipeFood() { Food = _bbqSauce, Amount = 10 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorWorstCaseNested_Fail()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 1 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _frozenChicken, Amount = 120 },
                new RecipeFood() { Food = _bbqSauce, Amount = 10 },
            };
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void MakeMultiple()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 10 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = null;
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook);
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void MakeMultiple_Bad()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _slicedBread, Amount = 10 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = null;
            for (var i = 0; i < 5; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook);
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void MakeMultipleMakeBread()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = null;
            for (var i = 0; i < 4; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook);
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void MakeMultipleMakeBread_Bad()
        {
            List<RecipeFood> pantry = new()
            {
                new RecipeFood() { Food = _eggs, Amount = 120 },
                new RecipeFood() { Food = _flour, Amount = 210 },
                new RecipeFood() { Food = _milk, Amount = 210 },
                new RecipeFood() { Food = _cookedChicken, Amount = 500 },
            };
            PantryProvider pp = new(pantry);
            Recipe recipe = _recipes.First(x => x.RecipeFoods.Any(y => y.Food == _chickenSandwich));
            CookPlan canCook = null;
            for (var i = 0; i < 5; i++)
            {
                canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
                canCook.ConsoleResult();
                pp.AdjustOnHandQuantity(canCook);
            }
            pp.GetFoodInstances().OutputRemaining();
            Assert.IsNotNull(canCook);
            Assert.IsFalse(canCook.CanMake);
        }
    }
}

