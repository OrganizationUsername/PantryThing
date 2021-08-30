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
        public CookPlan GetCookPlan(IList<FoodInstance> foodInventory, BetterRecipe recipe, IList<BetterRecipe> recipes)
        {
            var totalOutput = new List<FoodInstance>();
            var totalInput = new List<FoodInstance>();
            var clonedFoodInventory = CloneFoodInstances(foodInventory);
            var recipeLines = GetFoodInstancesFromRecipe(recipe);
            foreach (var foodInstance in recipeLines)
            {
                bool onlyPantryUsed = true;
                while (foodInstance.Amount > 0)
                {
                    var target = clonedFoodInventory.FirstOrDefault(x => x.FoodType == foodInstance.FoodType && x.Amount > 0);
                    if (target is not null)
                    {
                        double diminishAmount = Math.Min(foodInstance.Amount, target.Amount);
                        foodInstance.Amount -= diminishAmount;
                        target.Amount -= diminishAmount;
                        if (onlyPantryUsed)
                        {
                            totalInput.Add(new FoodInstance() { Amount = diminishAmount, FoodType = foodInstance.FoodType });
                        }
                        else
                        {
                            //Reduce total output.
                            //Null because I didn't deep copy anything.
                            //var OutputToReduce = totalOutput.FirstOrDefault(x => x.FoodType == foodInstance.FoodType && x.Amount > 0);
                            //OutputToReduce.Amount -= diminishAmount;
                        }
                        continue;
                    }
                    else
                    {
                        onlyPantryUsed = false;
                        var newRecipe = recipes.FirstOrDefault(x => x.MainOutput == foodInstance.FoodType);
                        if (newRecipe is null)
                        {
                            return new CookPlan() { CanMake = false };
                        }
                        else
                        {
                            var result = this.GetCookPlan(CloneFoodInstances(clonedFoodInventory), newRecipe, recipes);
                            if (result.CanMake)
                            {
                                totalInput.AddRange(result.TotalInput);
                                clonedFoodInventory.AddRange(result.TotalOutPut);
                                totalOutput.AddRange(result.TotalOutPut);
                            }
                            else
                            {
                                return new CookPlan() { CanMake = false };
                            }
                        }
                    }
                }

            }
            totalOutput.AddRange(recipe.Outputs);
            return new CookPlan() { CanMake = true, TotalOutPut = totalOutput, TotalInput = totalInput };
        }
        private static FoodInstance[] GetFoodInstancesFromRecipe(BetterRecipe recipe)
        {
            FoodInstance[] clones = new FoodInstance[recipe.Inputs.Count];
            for (var index = 0; index < recipe.Inputs.Count; index++)
            {
                FoodInstance fi = recipe.Inputs[index];
                clones[index] = (new FoodInstance()
                {
                    FoodType = fi.FoodType,
                    Amount = fi.Amount
                });
            }
            return clones;
        }
        private static List<FoodInstance> CloneFoodInstances(IList<FoodInstance> foodInstances)
        {
            var clones = new FoodInstance[foodInstances.Count];
            for (var index = 0; index < foodInstances.Count; index++)
            {
                FoodInstance fi = foodInstances[index];
                clones[index] = (new FoodInstance()
                {
                    FoodType = fi.FoodType,
                    Amount = fi.Amount
                });
            }
            return clones.ToList();
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
                new Core.BetterRecipe()
                {
                    MainOutput = _cookedChicken,
                    Inputs = new List<FoodInstance>() {
                        new(){Amount = 120,FoodType = _rawChicken },
                        new(){Amount = 1,FoodType = _bbqSauce } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _cookedChicken } },
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
                    MainOutput = _rawChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _frozenChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _rawChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Put chicken in fridge.", TimeCost = 1,Equipments = new List<Equipment>() {_fridge, _humanMachine}},
                        new() {Instruction = "Let it defrost.", TimeCost = 1440, Equipments = new List<Equipment>() {_fridge}}
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = _slicedChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _cookedChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _slicedChicken } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Cut chicken with a knife.", TimeCost = 3, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = _slicedBread,
                    Inputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _bread } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 10, FoodType = _slicedBread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = _chickenSandwich,
                    Inputs = new List<FoodInstance>() {
                        new() { Amount = 2, FoodType = _slicedBread } ,
                        new() { Amount = 120, FoodType = _slicedChicken} },
                    Outputs = new List<FoodInstance>() { new() { Amount = 10, FoodType = _chickenSandwich } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Assemble Sandwich", TimeCost = 1, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    MainOutput = _bread,
                    Inputs = new List<FoodInstance>() {
                        new() { Amount = 120, FoodType = _eggs } ,
                        new() { Amount = 120, FoodType = _milk} ,
                        new() { Amount = 120, FoodType = _flour } ,
                    },
                    Outputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _bread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Insert into Bread Machine.", TimeCost = 1, Equipments = new(){_humanMachine, _breadMachine}},
                        new() {Instruction = "Bread Machine cooks.", TimeCost = 180, Equipments = new(){ _breadMachine}},
                        new() {Instruction = "Extract bread from bread machine.", TimeCost = 1, Equipments = new(){_humanMachine, _breadMachine}},
                    }
                });
        }

        [Test]
        public void BetterFoodProcessorSimple()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _slicedBread, Amount = 10 },
                new FoodInstance() { FoodType = _slicedChicken, Amount = 120 },
            };
            BetterRecipe recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorSimple_InsufficientBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _slicedBread, Amount = 1 },
                new FoodInstance() { FoodType = _slicedChicken, Amount = 120 },
            };
            BetterRecipe recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorSimpleNested()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _slicedBread, Amount = 10 },
                new FoodInstance() { FoodType = _cookedChicken, Amount = 120 },
            };
            BetterRecipe recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }


    }

}

