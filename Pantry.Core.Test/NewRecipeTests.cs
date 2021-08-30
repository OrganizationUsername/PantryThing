using System;
using System.Collections;
using Pantry.Core.FoodProcessing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Pantry.Core.Scheduler;

namespace Pantry.Core.Test
{
    public class BetterFoodProcessor
    {
        public CookPlan GetCookPlan(IList<FoodInstance> foodInventory, BetterRecipe recipe, IList<BetterRecipe> recipes)
        {
            var recipeSteps = new Queue<List<RecipeStep>>();
            var steps = "";//$"\"Start {recipe.MainOutput.Name}\"";
            recipeSteps.Enqueue(recipe.RecipeSteps);
            var totalOutput = new List<FoodInstance>();
            var totalInput = new List<FoodInstance>();
            var clonedFoodInventory = CloneFoodInstances(foodInventory);
            var recipeLines = GetFoodInstancesFromRecipe(recipe);
            foreach (var foodInstance in recipeLines)
            {
                bool onlyPantryUsed = true;
                while (foodInstance.Amount > 0)
                {
                    var target = clonedFoodInventory.FirstOrDefault(x => x.FoodType.FoodId == foodInstance.FoodType.FoodId && x.Amount > 0);
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
                            var outputToReduce = totalOutput.First(x => x.FoodType.FoodId == foodInstance.FoodType.FoodId && x.Amount > 0);
                            outputToReduce.Amount -= diminishAmount;
                        }
                        continue;
                    }
                    onlyPantryUsed = false;
                    var newRecipe = recipes.FirstOrDefault(x => x.MainOutput.FoodId == foodInstance.FoodType.FoodId);
                    if (newRecipe is null)
                    {
                        return new CookPlan() { CanMake = false };
                    }

                    var result = this.GetCookPlan(CloneFoodInstances(clonedFoodInventory), newRecipe, recipes);
                    if (result.CanMake)
                    {
                        totalInput.AddRange(result.TotalInput);
                        clonedFoodInventory.AddRange(CloneFoodInstances(result.TotalOutput));
                        totalOutput.AddRange(CloneFoodInstances(result.TotalOutput));
                        recipeSteps.Enqueue(result.RecipeSteps.SelectMany(x => x).ToList()); //This is very wrong.
                        steps += $"{Environment.NewLine}{result.Steps}-> \"{recipe.MainOutput.Name}\"";
                    }
                    else
                    {
                        return new CookPlan() { CanMake = false };
                    }
                }
            }
            totalOutput.AddRange(recipe.Outputs);
            if (string.IsNullOrWhiteSpace(steps))
            {
                steps = $"\"{recipe.MainOutput.Name}\"";
            }
            return new CookPlan()
            {
                CanMake = true,
                TotalOutput = totalOutput,
                TotalInput = totalInput,
                RecipeSteps = recipeSteps,
                Steps = steps// + $"[End {recipe.MainOutput.Name}]"
            };
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
                    FoodType = new Food() { Name = fi.FoodType.Name, FoodId = fi.FoodType.FoodId },
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
                    RecipeId = 1,
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
                    RecipeId = 2,
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
                    RecipeId = 3,
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
                    RecipeId = 4,
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
                    RecipeId = 5,
                    MainOutput = _chickenSandwich,
                    Inputs = new List<FoodInstance>() {
                        new() { Amount = 2, FoodType = _slicedBread } ,
                        new() { Amount = 120, FoodType = _slicedChicken} },
                    Outputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _chickenSandwich } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new() {Instruction = "Assemble Sandwich", TimeCost = 1, Equipments = new List<Equipment>(){_humanMachine}},
                    }
                });
            _recipes.Add(
                new Core.BetterRecipe()
                {
                    RecipeId = 6,
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

        [Test]
        public void BetterFoodProcessorComplexNested()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _bread, Amount = 10 },
                new FoodInstance() { FoodType = _cookedChicken, Amount = 120 },
            };
            BetterRecipe recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }


        [Test]
        public void BetterFoodProcessorBadCaseNested()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _eggs, Amount = 210 },
                new FoodInstance() { FoodType = _flour, Amount = 210 },
                new FoodInstance() { FoodType = _milk, Amount = 210 },
                new FoodInstance() { FoodType = _cookedChicken, Amount = 120 },
            };
            BetterRecipe recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void BetterFoodProcessorWorstCaseNested()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _eggs, Amount = 210 },
                new FoodInstance() { FoodType = _flour, Amount = 210 },
                new FoodInstance() { FoodType = _milk, Amount = 210 },
                new FoodInstance() { FoodType = _frozenChicken, Amount = 120 },
                new FoodInstance() { FoodType = _bbqSauce, Amount = 10 },
            };
            BetterRecipe recipe = _recipes.First(x => x.MainOutput == _chickenSandwich);
            CookPlan canCook = _foodProcessor.GetCookPlan(pantry, recipe, _recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

    }

}

