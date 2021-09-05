using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pantry.Core.Extensions;
using Pantry.Core.FoodProcessing;
using Pantry.Core.Models;

namespace Pantry.Models.Core.Test
{
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
        private readonly IFoodProcessor _foodProcessor = new SimpleFoodProcessor();
        [SetUp]
        public void Setup()
        {
            Recipes = new List<Recipe>
            {
                new Recipe()
                {
                    RecipeId = 1,
                    Description = "Cheese sandwich",
                    InputFoodInstance =
                        new List<FoodInstance>()
                        {
                            new() {FoodType = _bread, Amount = 2}, new() {FoodType = _cheese, Amount = 1},
                        },
                    OutputFoodInstance = new FoodInstance() {FoodType = _cheeseSandwich, Amount = 1},
                    TimeCost = 3,
                },
                new Recipe()
                {
                    RecipeId = 2,
                    Description = "Bread",
                    InputFoodInstance =
                        new List<FoodInstance>()
                        {
                            new() {FoodType = _flour, Amount = 2}, new() {FoodType = _milk, Amount = 1},
                        },
                    OutputFoodInstance = new FoodInstance() {FoodType = _bread, Amount = 1},
                    TimeCost = 45,
                },
                new Recipe()
                {
                    RecipeId = 3,
                    Description = "Full Meal",
                    InputFoodInstance = new List<FoodInstance>()
                    {
                        new() {FoodType = _cheeseSandwich, Amount = 1}, new() {FoodType = _milk, Amount = 1},
                    },
                    OutputFoodInstance = new FoodInstance() {FoodType = _cheeseSandwichMeal, Amount = 1},
                    TimeCost = 1,
                },
                new Recipe()
                {
                    RecipeId = 4,
                    Description = "Grilled Cheese Meal",
                    InputFoodInstance = new List<FoodInstance>()
                    {
                        new() {FoodType = _grilledCheese, Amount = 1}, new() {FoodType = _milk, Amount = 1},
                    },
                    OutputFoodInstance = new FoodInstance() {FoodType = _cheeseSandwichMeal, Amount = 1},
                    TimeCost = 1,
                },
                new Recipe()
                {
                    RecipeId = 5,
                    Description = "Unobtainable Meal",
                    InputFoodInstance = new List<FoodInstance>()
                    {
                        new() {FoodType = _grilledCheese, Amount = 1},
                        new() {FoodType = _unobtainableTomatoSoup, Amount = 1},
                    },
                    OutputFoodInstance = new FoodInstance() {FoodType = _unobtainableMeal, Amount = 1},
                    TimeCost = 1,
                }
            };
        }

        [Test]
        public void CannotMakeUnobtainableMeal()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 4 },
                new FoodInstance() { FoodType = _milk, Amount = 3 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
                new FoodInstance() { FoodType = _bread, Amount = 10 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _unobtainableMeal);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CanMakeMealWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 4 },
                new FoodInstance() { FoodType = _milk, Amount = 3 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CanMakeMealWithMixedFractionalBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 4 },
                new FoodInstance() { FoodType = _milk, Amount = 3 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
                new FoodInstance() { FoodType = _bread, Amount = 0.25 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
            Assert.AreEqual(2, canCook.RecipesTouched.Count(x => x.Description == "Bread"));
        }

        [Test]
        public void CanMakeMealWithFractionalBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 4 },
                new FoodInstance() { FoodType = _milk, Amount = 3 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
                new FoodInstance() { FoodType = _bread, Amount = 1.25 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
            Assert.AreEqual(1, canCook.RecipesTouched.Count(x => x.Description == "Bread"));
        }

        [Test]
        public void CanMakeMealWitHalfBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 2 },
                new FoodInstance() { FoodType = _milk, Amount = 3 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
                new FoodInstance() { FoodType = _bread, Amount = 1 },

            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CannotMakeMealWithoutSufficientFlour()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 3 },
                new FoodInstance() { FoodType = _milk, Amount = 3 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CannotMakeMealWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 2 },
                new FoodInstance() { FoodType = _milk, Amount = 1 },
                new FoodInstance() { FoodType = _cheese, Amount = 2 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwichMeal);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CanMakeCheeseSandwichWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 4 },
                new FoodInstance() { FoodType = _milk, Amount = 2 },
                new FoodInstance() { FoodType = _cheese, Amount = 2 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwichWithoutBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 1 },
                new FoodInstance() { FoodType = _milk, Amount = 1 },
                new FoodInstance() { FoodType = _cheese, Amount = 2 },
                new FoodInstance() { FoodType = _bread, Amount = 0 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwichWithNoBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 1 },
                new FoodInstance() { FoodType = _milk, Amount = 1 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwichWithOnlyOneBread()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _flour, Amount = 2 },
                new FoodInstance() { FoodType = _milk, Amount = 1 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe, Recipes);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }

        [Test]
        public void CanMakeCheeseSandwich()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _bread, Amount = 2 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe);
            Assert.AreEqual(1, canCook.TotalInput.Single(x => x.FoodType == _cheese).Amount);
            Assert.AreEqual(2, canCook.TotalInput.Single(x => x.FoodType == _bread).Amount);
            Assert.AreEqual(1, canCook.TotalOutput.Single(x => x.FoodType == _cheeseSandwich).Amount);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CanMakeCheeseSandwichSplitted()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _bread, Amount = 1 },
                new FoodInstance() { FoodType = _bread, Amount = 1 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
            };

            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe);
            canCook.ConsoleResult();
            Assert.IsTrue(canCook.CanMake);
        }

        [Test]
        public void CannotMakeCheeseSandwich()
        {
            List<FoodInstance> pantry = new()
            {
                new FoodInstance() { FoodType = _bread, Amount = 1 },
                new FoodInstance() { FoodType = _cheese, Amount = 1 },
            };
            var recipe = Recipes.First(r => r.OutputFoodInstance.FoodType == _cheeseSandwich);
            var canCook = _foodProcessor.CanCookSomething(pantry, recipe);
            canCook.ConsoleResult();
            Assert.IsFalse(canCook.CanMake);
        }
    }
}