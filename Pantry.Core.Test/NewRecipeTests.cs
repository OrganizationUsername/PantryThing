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
        public GetCookPlan CanCookSomething(IList<FoodInstance> foodInventory, Recipe recipe,
            IList<Recipe> recipes = null)
        {
            throw new NotImplementedException();
        }
    }

    public class BetterRecipe
    {
        public List<FoodInstance> Inputs { get; set; }
        public List<FoodInstance> Outputs { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
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


        public List<Recipe> Recipes;






        private List<Equipment> _equipments;
        private readonly BetterFoodProcessor _foodProcessor = new BetterFoodProcessor();

        [SetUp]
        public void Setup()
        {
            Food _frozenChicken = new Food() { FoodId = 0, Name = "Frozen Chicken", BetterRecipes = null };


            Food _bbqSauce = new Food() { FoodId = 2, Name = "BBQ Sauce", BetterRecipes = null };
            Food _cookedChicken = new Food() { FoodId = 3, Name = "Cooked Chicken" };
            Food _slicedChicken = new Food() { FoodId = 4, Name = "Sliced Chicken" };
            Food _flour = new Food() { FoodId = 5, Name = "Flour" };
            Food _eggs = new Food() { FoodId = 6, Name = "Eggs" };
            Food _milk = new Food() { FoodId = 7, Name = "Milk" };
            Food _bread = new Food() { FoodId = 8, Name = "Bread" };
            Food _slicedBread = new Food() { FoodId = 9, Name = "Sliced Bread" };
            Food _chickenSandwich = new Food() { FoodId = 10, Name = "Chicken Sandwich" };
            System.Diagnostics.Process.Start("ArbitraryProgram.exe");

            Food _rawChicken = new Food()
            {
                FoodId = 1,
                Name = "Raw Chicken",
            };
            _rawChicken.BetterRecipes = new List<Core.BetterRecipe>()
            {
                new Core.BetterRecipe()
                {
                    Inputs = new List<FoodInstance>()
                    {
                        new FoodInstance(){Amount = 120,FoodType = _rawChicken}
                    },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new RecipeStep() {Instruction = "Put chicken in fridge.", TimeCost = 1},
                        new RecipeStep()
                        {
                            Instruction = "Let it defrost.", TimeCost = 1440,
                            Equipments = new List<Equipment>() {_fridge}
                        }
                    }
                }
            };



        }
    }

}






/////
//Recipe recipe = new Recipe()
//    {
//        RecipeId = 1,
//        Description = "Chicken Sandwich",
//        InputFoodInstance = new List<FoodInstance>()
//        {
//            new() {FoodType = _slicedChicken, Amount = 120},
//            new() {FoodType = _slicedBread, Amount = 2},
//        },
//        OutputFoodInstance = new FoodInstance() { FoodType = _chickenSandwich, Amount = 1 },
//        TimeCost = 1,
//        RecipeSteps = new List<RecipeStep>()
//        {
//            new()
//            {
//                Order = 1,
//                Instruction = "Assemble Sandwich.",
//                TimeCost = 3,
//                Equipments = new List<Equipment>() {_humanMachine}
//            },
//        },
//        RecipeHierarchy = new RecipeHierarchy()
//        {
//            Instruction = "Assemble Sandwich",
//            TimeCost = 3,
//            Equipments = new List<Equipment>() { _humanMachine }
//        },
//    };

//    Recipes = new List<Recipe>
//    {
//        new Recipe()
//        {
//            RecipeId = 1,
//            Description = "Chicken Sandwich",
//            InputFoodInstance = new List<FoodInstance>()
//            {
//                new() {FoodType = _slicedChicken, Amount = 120},
//                new() {FoodType = _slicedBread, Amount = 2},
//            },
//            OutputFoodInstance = new FoodInstance() {FoodType = _cheeseSandwich, Amount = 1},
//            TimeCost = 3,
//            RecipeSteps = new List<RecipeStep>()
//            {
//                new()
//                {
//                    Order = 1,
//                    Instruction = "Assemble Sandwich.",
//                    TimeCost = 3,
//                    Equipments = new List<Equipment>() {_humanMachine}
//                },
//            },
//            RecipeHierarchy = new RecipeHierarchy()
//            {
//                Instruction = "Assemble Sandwich",
//                TimeCost = 3,
//                Equipments = new List<Equipment>() {_humanMachine},
//                Dependents = new List<RecipeHierarchy>() { }
//            },
//        },
//        new Recipe()
//        {
//            RecipeId = 2,
//            Description = "Bread",
//            InputFoodInstance = new List<FoodInstance>()
//            {
//                new() {FoodType = _flour, Amount = 2},
//                new() {FoodType = _milk, Amount = 1},
//            },
//            OutputFoodInstance = new FoodInstance() {FoodType = _bread, Amount = 10},
//            TimeCost = 45,
//            RecipeSteps = new List<RecipeStep>()
//            {
//                new()
//                {
//                    Order = 1, Instruction = "Fill/operate bread machine.", TimeCost = 1,
//                    Equipments = new List<Equipment>() {_breadMachine, _humanMachine}
//                },
//                new()
//                {
//                    Order = 2, Instruction = "Do its thing.", TimeCost = 40,
//                    Equipments = new List<Equipment>() {_breadMachine}
//                },
//            },
//            RecipeHierarchy = new RecipeHierarchy()
//            {
//                Instruction = "Do its thing.",
//                TimeCost = 40,
//                Equipments = new List<Equipment>() {_breadMachine},
//                Dependents = new List<RecipeHierarchy>()
//                {
//                    new RecipeHierarchy()
//                    {
//                        Instruction = "Fill/operate bread machine.",
//                        TimeCost = 1,
//                        Equipments = new List<Equipment>() {_breadMachine, _humanMachine},
//                        Dependents = new List<RecipeHierarchy>() { }
//                    }
//                }
//            },
//        },
//        new Recipe()
//        {
//            RecipeId = 10,
//            Description = "CookedRice",
//            InputFoodInstance = new List<FoodInstance>()
//            {
//                new() {FoodType = _rice, Amount = 1},
//                new() {FoodType = _water, Amount = 3},
//            },
//            OutputFoodInstance = new FoodInstance() {FoodType = _cookedRice, Amount = 4},
//            TimeCost = 60,
//            RecipeSteps = new List<RecipeStep>()
//            {
//                new()
//                {
//                    Order = 1, Instruction = "Fill/operate rice machine.", TimeCost = 1,
//                    Equipments = new List<Equipment>() {_riceMachine, _humanMachine}
//                },
//                new()
//                {
//                    Order = 2, Instruction = "Do its thing.", TimeCost = 40,
//                    Equipments = new List<Equipment>() {_riceMachine}
//                },
//            },
//            RecipeHierarchy = new RecipeHierarchy()
//            {
//                Instruction = "Do its thing.",
//                TimeCost = 40,
//                Equipments = new List<Equipment>() {_riceMachine},
//                Dependents = new List<RecipeHierarchy>()
//                {
//                    new RecipeHierarchy()
//                    {
//                        Instruction = "Fill/operate rice machine.",
//                        TimeCost = 1,
//                        Equipments = new List<Equipment>() {_riceMachine, _humanMachine},
//                        Dependents = new List<RecipeHierarchy>() { }
//                    }
//                }
//            },
//        },
//        new Recipe()
//        {
//            RecipeId = 11,
//            Description = "CurriedRice",
//            InputFoodInstance = new List<FoodInstance>()
//            {
//                new() {FoodType = _cookedRice, Amount = 4},
//                new() {FoodType = _currySauce, Amount = 2},
//            },
//            OutputFoodInstance = new FoodInstance() {FoodType = _curryMeal, Amount = 6},
//            TimeCost = 5,
//            RecipeSteps = new List<RecipeStep>()
//            {
//                new()
//                {
//                    Order = 1, Instruction = "Heat and plate.", TimeCost = 5,
//                    Equipments = new List<Equipment>() {_stoveTop, _humanMachine}
//                },
//            },
//            RecipeHierarchy = new RecipeHierarchy()
//            {
//                Instruction = "Heat and plate.",
//                TimeCost = 5,
//                Equipments = new List<Equipment>() {_stoveTop, _humanMachine},
//                Dependents = new List<RecipeHierarchy>()
//            },
//        }
//    };
//    _equipments = new List<Equipment>() { _breadMachine, _humanMachine, _riceMachine, _stoveTop };



//List<FoodInstance> pantry = new()
//{
//    new FoodInstance() { FoodType = _flour, Amount = 4 },
//    new FoodInstance() { FoodType = _milk, Amount = 3 },
//    new FoodInstance() { FoodType = _cheese, Amount = 1 },
//    new FoodInstance() { FoodType = _bread, Amount = 1 },
//    new FoodInstance() { FoodType = _water, Amount = double.MaxValue },
//    new FoodInstance() { FoodType = _rice, Amount = 100 },
//    new FoodInstance() { FoodType = _currySauce, Amount = 10 },
//};
//var pp = new PantryProvider(pantry);
//var cms = new List<GetCookPlan>();
//Recipe recipe = default;
//GetCookPlan canCook = default;
//recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
//canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
//cms.Add(canCook);
//canCook.ConsoleResult();
//pantry = pp.DiminishFood(canCook);
//Assert.IsTrue(canCook.CanMake);
//recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _curryMeal);
//canCook = _foodProcessor.CanCookSomething(pp.GetFoodInstances(), recipe, Recipes);
//cms.Add(canCook);
//canCook.ConsoleResult();
//pantry = pp.DiminishFood(canCook);
//pp.GetFoodInstances().OutputRemaining();
//Assert.IsTrue(canCook.CanMake);
//Console.WriteLine("-----");

//foreach (var canMakeSomething in cms)
//{
//    Console.WriteLine(string.Join(Environment.NewLine,
//        canMakeSomething.RecipesTouched.Select(y =>
//            y.Description + ": " + y.RecipeSteps.Sum(z => z.TimeCost))));
//    Console.WriteLine("-----");
//}

//IScheduler simpleScheduler = new NaiveScheduler(cms, _equipments);
//simpleScheduler.TrySchedule(DateTime.Parse("2021/08/15 18:00"));
