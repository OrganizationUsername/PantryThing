using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Pantry.Core;

namespace Pantry.Core.Test
{
    public static class ExtensionMethods
    {
        public static void ConsoleResult(this CanMakeSomething canCook)
        {
            if (canCook.CanMake)
            {
                Console.WriteLine(string.Join(Environment.NewLine, canCook.TotalCost.Select(x => x.FoodType.Name)));
                Console.WriteLine($"Recipes: {Environment.NewLine}" + string.Join(Environment.NewLine, canCook.RecipesTouched.Select(x => x.Description)));
                //Console.WriteLine($"Time Taken: {canCook.RecipesTouched.Sum(x => x.RecipeSteps.Sum(y => y.TimeCost))}");
                Console.WriteLine($"Total Cost:{Environment.NewLine}"
                                  + string.Join(Environment.NewLine, canCook.RawCost.Select(x => x.FoodType.Name + ": " + x.Amount)));
                Console.WriteLine("-----");
            }
        }

        public static List<FoodInstance> DiminishFoodInstances(this List<FoodInstance> pantry, CanMakeSomething canCook)
        {
            if (canCook.CanMake)
            {
                foreach (var rawCost in canCook.RawCost)
                {
                    while (rawCost.Amount > 0)
                    {
                        var pantryItem =
                            pantry.First(pantry => pantry.FoodType == rawCost.FoodType && pantry.Amount > 0);
                        var AmountToDeduct = Math.Min(pantryItem.Amount, rawCost.Amount);
                        pantryItem.Amount -= AmountToDeduct;
                        rawCost.Amount -= AmountToDeduct;
                    }
                }

                return pantry.Where(pi => pi.Amount > 0).ToList();
            }
            else
            {
                return null;
            }
        }
        public static void OutputRemaining(this List<FoodInstance> pantry)
        {
            if (pantry is null)
            {
                return;
            }
            Console.WriteLine($"Remaining: {Environment.NewLine}" + string.Join(Environment.NewLine, pantry.Where(pi => pi.Amount > 0 && pi.Amount < 10_000_000).Select(pi => pi.FoodType.Name + ": " + pi.Amount)));
        }
    }

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
        private readonly Equipment _breadMachine = new() { Name = "Bread Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime)>() };
        private readonly Equipment _riceMachine = new() { Name = "Rice Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime)>() };
        private readonly Equipment _humanMachine = new() { Name = "Human", BookedTimes = new List<(DateTime startTime, DateTime endTime)>() };
        private readonly Equipment _stoveTop = new() { Name = "StoveTop", BookedTimes = new List<(DateTime startTime, DateTime endTime)>() };


        [SetUp]
        public void Setup()
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            Recipes = new List<Recipe>();
#pragma warning restore IDE0028 // Simplify collection initialization
            Recipes.Add(new Recipe()
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
                    new(){Order = 1,Instruction = "Assemble Sandwich.", TimeCost = 3, Equipments = new List<Equipment>(){_humanMachine}},
                },
            });
            Recipes.Add(new Recipe()
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
                        new(){Order = 2,Instruction = "Wait.", TimeCost = 40, Equipments = new List<Equipment>(){_breadMachine}},
                },
            });
            Recipes.Add(new Recipe()
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
                    new(){Order = 2,Instruction = "Wait.", TimeCost = 40, Equipments = new List<Equipment>() {_riceMachine}},
                },
            });
            Recipes.Add(new Recipe()
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
            });
        }

        [Test]
        public void ScheduleMealsBeforeATime()
        {
            Scheduler scheduler = new Scheduler();

            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 4 },
                new() { FoodType = _milk, Amount = 3 },
                new() { FoodType = _cheese, Amount = 1 },
                new() { FoodType = _bread, Amount = 1 },
                new() { FoodType = _water, Amount = double.MaxValue },
                new() { FoodType = _rice, Amount = 100 },
                new() { FoodType = _currySauce, Amount = 10 },
            };
            Recipe recipe = default;
            CanMakeSomething canCook = default;
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            scheduler.Things.Add(canCook);
            canCook.ConsoleResult();
            pantry = pantry.DiminishFoodInstances(canCook);
            Assert.IsTrue(canCook.CanMake);
            recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _curryMeal);
            canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            scheduler.Things.Add(canCook);
            canCook.ConsoleResult();
            pantry = pantry.DiminishFoodInstances(canCook);
            pantry.OutputRemaining();
            Assert.IsTrue(canCook.CanMake);
            Console.WriteLine("-----");
            foreach (var canMakeSomething in scheduler.Things)
            {
                Console.WriteLine(string.Join(Environment.NewLine, canMakeSomething.RecipesTouched.Select(y => y.Description + ": " + y.RecipeSteps.Sum(z => z.TimeCost))));
                Console.WriteLine("-----");
            }
            scheduler.Equipments.AddRange(new List<Equipment>() { _breadMachine, _riceMachine, _humanMachine, _stoveTop });
            scheduler.TrySchedule(DateTime.Parse("2021/08/15 18:00"));
        }
    }


    public class RecipeTests
    {
        public List<Recipe> Recipes;
        private readonly Food _bread = new() { FoodId = 1, Name = "BreadSlice" };
        private readonly Food _cheese = new() { FoodId = 2, Name = "CheeseSlice" };
        private readonly Food _flour = new() { FoodId = 3, Name = "Flour" };
        private readonly Food _milk = new() { FoodId = 4, Name = "Milk" };
        private readonly Food _cheeseSandwich = new() { FoodId = 5, Name = "CheeseSandwich" };
        private readonly Food _cheeseSandwichMeal = new() { FoodId = 6, Name = "FullCheeseSandwichMeal" };
        private readonly Food _grilledCheese = new() { FoodId = 7, Name = "GrilledCheese" };
        private readonly Food _unobtainableTomatoSoup = new() { FoodId = 8, Name = "TomatoSoupWithoutRecipe" };
        private readonly Food _unobtainableMeal = new() { FoodId = 9, Name = "UnobtainableMeal" };

        [SetUp]
        public void Setup()
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            Recipes = new List<Recipe>();
#pragma warning restore IDE0028 // Simplify collection initialization
            Recipes.Add(new Recipe()
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
            });
            Recipes.Add(new Recipe()
            {
                RecipeId = 2,
                Description = "Bread",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _flour, Amount = 2 },
                    new() { FoodType = _milk, Amount = 1 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _bread, Amount = 1 },
                TimeCost = 45,
            });
            Recipes.Add(new Recipe()
            {
                RecipeId = 3,
                Description = "Full Meal",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _cheeseSandwich, Amount = 1 },
                    new() { FoodType = _milk, Amount = 1 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _cheeseSandwichMeal, Amount = 1 },
                TimeCost = 1,
            });
            Recipes.Add(new Recipe()
            {
                RecipeId = 4,
                Description = "Grilled Cheese Meal",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _grilledCheese, Amount = 1 },
                    new() { FoodType = _milk, Amount = 1 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _cheeseSandwichMeal, Amount = 1 },
                TimeCost = 1,
            });
            Recipes.Add(new Recipe()
            {
                RecipeId = 5,
                Description = "Unobtainable Meal",
                InputFoodInstance = new List<FoodInstance>()
                {
                    new() { FoodType = _grilledCheese, Amount = 1 },
                    new() { FoodType = _unobtainableTomatoSoup, Amount = 1 },
                },
                OutputFoodInstance = new FoodInstance() { FoodType = _unobtainableMeal, Amount = 1 },
                TimeCost = 1,
            });
        }

        [Test]
        public void CannotMakeUnobtainableMeal()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 4 },
                new() { FoodType = _milk, Amount = 3 },
                new() { FoodType = _cheese, Amount = 1 },
                new() { FoodType = _bread, Amount = 10 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _unobtainableMeal);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CanMakeMealWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 4 },
                new() { FoodType = _milk, Amount = 3 },
                new() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CanMakeMealWithMixedFractionalBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 4 },
                new() { FoodType = _milk, Amount = 3 },
                new() { FoodType = _cheese, Amount = 1 },
                new() { FoodType = _bread, Amount = 0.25 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
            Assert.AreEqual(2, canCook.RecipesTouched.Count(x => x.Description == "Bread"));
        }

        [Test]
        public void CanMakeMealWithFractionalBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 4 },
                new() { FoodType = _milk, Amount = 3 },
                new() { FoodType = _cheese, Amount = 1 },
                new() { FoodType = _bread, Amount = 1.25 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
            Assert.AreEqual(1, canCook.RecipesTouched.Count(x => x.Description == "Bread"));
        }

        [Test]
        public void CanMakeMealWitHalfBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 2 },
                new() { FoodType = _milk, Amount = 3 },
                new() { FoodType = _cheese, Amount = 1 },
                new() { FoodType = _bread, Amount = 1 },

            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CannotMakeMealWithoutSufficientFlour()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 3 },
                new() { FoodType = _milk, Amount = 3 },
                new() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CannotMakeMealWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 2 },
                new() { FoodType = _milk, Amount = 1 },
                new() { FoodType = _cheese, Amount = 2 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CanMakeCheeseSandwichWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 4 },
                new() { FoodType = _milk, Amount = 2 },
                new() { FoodType = _cheese, Amount = 2 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwichWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 1 },
                new() { FoodType = _milk, Amount = 1 },
                new() { FoodType = _cheese, Amount = 2 },
                new() { FoodType = _bread, Amount = 0 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwichWithNoBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 1 },
                new() { FoodType = _milk, Amount = 1 },
                new() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwichWithOnlyOneBread()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _flour, Amount = 2 },
                new() { FoodType = _milk, Amount = 1 },
                new() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = CookSomething.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CanMakeCheeseSandwich()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _bread, Amount = 2 },
                new() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = CookSomething.CanCookSomething(pantry, recipe);
            Assert.AreEqual(1, canCook.TotalCost.Single(x => x.FoodType == _cheese).Amount);
            Assert.AreEqual(2, canCook.TotalCost.Single(x => x.FoodType == _bread).Amount);
            Assert.AreEqual(1, canCook.TotalOutPut.Single(x => x.FoodType == _cheeseSandwich).Amount);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CanMakeCheeseSandwichSplitted()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _bread, Amount = 1 },
                new() { FoodType = _bread, Amount = 1 },
                new() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = CookSomething.CanCookSomething(pantry, recipe);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwich()
        {
            List<FoodInstance> pantry = new()
            {
                new() { FoodType = _bread, Amount = 1 },
                new() { FoodType = _cheese, Amount = 1 },
            };
            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = CookSomething.CanCookSomething(pantry, recipe);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }
    }
}