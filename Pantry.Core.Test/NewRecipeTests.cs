using System;
using Pantry.Core.FoodProcessing;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pantry.Core.Scheduler;

namespace Pantry.Core.Test
{
    public class BetterFoodProcessor
    {
        public GetCookPlan CanCookSomething(IList<FoodInstance> foodInventory, BetterRecipe recipe, IList<BetterRecipe> recipes = null)
        {
            throw new NotImplementedException();
        }
    }

    public class NewRecipeTests
    {
        private readonly Equipment _breadMachine = new()
        { Name = "Bread Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private readonly Equipment _humanMachine = new()
        { Name = "Human", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private readonly Equipment _stoveTop = new()
        { Name = "StoveTop", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private readonly Equipment _fridge = new()
        { Name = "Fridge", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private readonly Equipment _sousVide = new()
        { Name = "Sous Vide", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };

        private List<Equipment> _equipments = new();
        private List<BetterRecipe> _recipes = new();
        private readonly BetterFoodProcessor _foodProcessor = new();
        Food frozenChicken = new Food() { FoodId = 0, Name = "Frozen Chicken", BetterRecipes = null };
        Food rawChicken = new Food() { FoodId = 1, Name = "Raw Chicken", };
        Food bbqSauce = new Food() { FoodId = 2, Name = "BBQ Sauce", BetterRecipes = null };
        Food cookedChicken = new Food() { FoodId = 3, Name = "Cooked Chicken" };
        Food slicedChicken = new Food() { FoodId = 4, Name = "Sliced Chicken" };
        Food flour = new Food() { FoodId = 5, Name = "Flour", BetterRecipes = null };
        Food eggs = new Food() { FoodId = 6, Name = "Eggs", BetterRecipes = null };
        Food milk = new Food() { FoodId = 7, Name = "Milk", BetterRecipes = null };
        Food bread = new Food() { FoodId = 8, Name = "Bread" };
        Food slicedBread = new Food() { FoodId = 9, Name = "Sliced Bread" };
        Food chickenSandwich = new Food() { FoodId = 10, Name = "Chicken Sandwich" };

        [SetUp]
        public void Setup()
        {


            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = cookedChicken,
                    Inputs = new List<FoodInstance>() {
                        new(){Amount = 120,FoodType = rawChicken },
                        new(){Amount = 1,FoodType = bbqSauce } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = cookedChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Put chicken in Sous Vide.", TimeCost = 1, Equipments = new List<Equipment>() {_sousVide, _humanMachine}},
                        new() {Instruction = "Let it cook.", TimeCost = 120, Equipments = new List<Equipment>() { _sousVide }},
                        new() {Instruction = "Take chicken out.", TimeCost = 1, Equipments = new List<Equipment>() {_sousVide, _humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = rawChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = frozenChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = rawChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Put chicken in fridge.", TimeCost = 1,Equipments = new List<Equipment>() {_fridge, _humanMachine}},
                        new() {Instruction = "Let it defrost.", TimeCost = 1440, Equipments = new List<Equipment>() {_fridge}}
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = slicedChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = cookedChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = slicedChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Cut chicken with a knife.", TimeCost = 3, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = slicedBread,
                    Inputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = bread } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 10, FoodType = slicedBread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = chickenSandwich,
                    Inputs = new List<FoodInstance>() {
                        new() { Amount = 2, FoodType = slicedBread } ,
                        new() { Amount = 120, FoodType = slicedChicken} },
                    Outputs = new List<FoodInstance>() { new() { Amount = 10, FoodType = chickenSandwich } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Assemble Sandwich", TimeCost = 1, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = bread,
                    Inputs = new List<FoodInstance>() {
                        new() { Amount = 120, FoodType = eggs } ,
                        new() { Amount = 120, FoodType = milk} ,
                        new() { Amount = 120, FoodType = flour } ,
                    },
                    Outputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = bread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Insert into Bread Machine.", TimeCost = 1, Equipments = new List<Equipment>(){_humanMachine, _breadMachine}},
                        new() {Instruction = "Bread Machine cooks.", TimeCost = 180, Equipments = new List<Equipment>(){ _breadMachine}},
                        new() {Instruction = "Extract bread from bread machine.", TimeCost = 1, Equipments = new List<Equipment>(){_humanMachine, _breadMachine}},
                    }
                });
        }

        [Test]
        public void BetterFoodProcessorSimple()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = slicedBread, Amount = 10 },
                new FoodInstance() { FoodType = slicedChicken, Amount = 120 },
            };
            BetterRecipe recipe = _recipes.First(x => x.MainOutput == chickenSandwich);
            GetCookPlan canCook = _foodProcessor.CanCookSomething(pantry, recipe, _recipes);
            Assert.IsTrue(canCook.CanMake);
        }


    }

}

