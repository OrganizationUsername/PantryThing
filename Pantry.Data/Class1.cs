using Pantry.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pantry.Data
{

    public class HardCodedFoodRepository
    {
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
        private List<Food> Foods { get; set; } = new List<Food>();

        public HardCodedFoodRepository()
        {
            Foods.Add(_frozenChicken);
            Foods.Add(_rawChicken);
            Foods.Add(_bbqSauce);
            Foods.Add(_cookedChicken);
            Foods.Add(_slicedChicken);
            Foods.Add(_flour);
            Foods.Add(_eggs);
            Foods.Add(_milk);
            Foods.Add(_bread);
            Foods.Add(_slicedBread);
            Foods.Add(_chickenSandwich);
        }
        public IList<Food> GetFoods()
        {
            return Foods;
        }
    }

    public class HardCodedRecipeRepository
    {
        private readonly List<Recipe> _recipes = new();
        private List<Equipment> _equipments;
        private readonly Equipment _breadMachine = new()
        { Name = "Bread Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _humanMachine = new()
        { Name = "Human", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _fridge = new()
        { Name = "Fridge", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
        private readonly Equipment _sousVide = new()
        { Name = "Sous Vide", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
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

        public HardCodedRecipeRepository()
        {
            HardCodedFoodRepository hcfr = new();
            var foods = hcfr.GetFoods();
            _recipes.Add(
             new Recipe()
             {
                 Id = 1,
                 MainOutput = _cookedChicken,
                 Inputs = new List<RecipeFood>()
                 {
                        new() {Amount = 120, Food = _frozenChicken, Id = _frozenChicken.FoodId},
                        new() {Amount = 1, Food = _bbqSauce , Id = _bbqSauce.FoodId },
                 },
                 Outputs = new List<RecipeFood>() { new() { Amount = 120, Food = _cookedChicken, Id = _cookedChicken.FoodId }, },
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
                new Recipe()
                {
                    Id = 2,
                    MainOutput = _rawChicken,
                    Inputs = new List<RecipeFood>() { new() { Amount = 120, Food = _frozenChicken, Id = _frozenChicken.FoodId }, },
                    Outputs = new List<RecipeFood>() { new() { Amount = 120, Food = _rawChicken, Id = _rawChicken.FoodId }, },
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
                new Recipe()
                {
                    Id = 3,
                    MainOutput = _slicedChicken,
                    Inputs = new List<RecipeFood>() { new() { Amount = 120, Food = _cookedChicken, Id = _cookedChicken.FoodId }, },
                    Outputs = new List<RecipeFood>() { new() { Amount = 120, Food = _slicedChicken, Id = _slicedChicken.FoodId }, },
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
                new Recipe()
                {
                    Id = 4,
                    MainOutput = _slicedBread,
                    Inputs = new List<RecipeFood>() { new() { Amount = 1, Food = _bread, Id = _bread.FoodId }, },
                    Outputs = new List<RecipeFood>() { new() { Amount = 10, Food = _slicedBread, Id = _slicedBread.FoodId }, },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>() {_humanMachine}
                        },
                    }
                });
            _recipes.Add(
                new Recipe()
                {
                    Id = 5,
                    MainOutput = _chickenSandwich,
                    Inputs = new List<RecipeFood>()
                    {
                        new() {Amount = 2, Food = _slicedBread, Id = _slicedBread.FoodId},
                        new() {Amount = 120, Food = _slicedChicken, Id = _slicedChicken.FoodId}
                    },
                    Outputs = new List<RecipeFood>()
                    {
                        new() { Amount = 1, Food = _chickenSandwich , Id = _chickenSandwich.FoodId}

                    },
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
                new Recipe()
                {
                    Id = 6,
                    MainOutput = _bread,
                    Inputs = new List<RecipeFood>()
                    {
                        new() {Amount = 120, Food = _eggs, Id = _eggs.FoodId},
                        new() {Amount = 120, Food = _milk, Id = _milk.FoodId},
                        new() {Amount = 120, Food = _flour, Id = _flour.FoodId},
                    },
                    Outputs = new List<RecipeFood>() { new() { Amount = 1, Food = _bread, Id = _bread.FoodId }, },
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
            _equipments = new List<Equipment>() { _breadMachine, _humanMachine, _fridge, _sousVide };

        }

        public IList<Recipe> GetRecipes()
        {
            return _recipes;
        }
    }
}
