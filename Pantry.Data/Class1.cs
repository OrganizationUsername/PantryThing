﻿using Pantry.Core.Models;
using System;
using System.Collections.Generic;

namespace Pantry.Data
{
    public interface IRecipeRepository
    {
        IList<BetterRecipe> GetRecipes();
    }

    public class HardCodedRecipeRepository : IRecipeRepository
    {
        private readonly List<BetterRecipe> _recipes = new();
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
            _recipes.Add(
             new BetterRecipe()
             {
                 RecipeId = 1,
                 MainOutput = _cookedChicken,
                 Inputs = new List<FoodInstance>()
                 {
                        new() {Amount = 120, FoodType = _rawChicken},
                        new() {Amount = 1, FoodType = _bbqSauce}
                 },
                 Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _cookedChicken } },
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
                new BetterRecipe()
                {
                    RecipeId = 2,
                    MainOutput = _rawChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _frozenChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _rawChicken } },
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
                new BetterRecipe()
                {
                    RecipeId = 3,
                    MainOutput = _slicedChicken,
                    Inputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _cookedChicken } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 120, FoodType = _slicedChicken } },
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
                new BetterRecipe()
                {
                    RecipeId = 4,
                    MainOutput = _slicedBread,
                    Inputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _bread } },
                    Outputs = new List<FoodInstance>() { new() { Amount = 10, FoodType = _slicedBread } },
                    RecipeSteps = new List<RecipeStep>()
                    {
                        new()
                        {
                            Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>() {_humanMachine}
                        },
                    }
                });
            _recipes.Add(
                new BetterRecipe()
                {
                    RecipeId = 5,
                    MainOutput = _chickenSandwich,
                    Inputs = new List<FoodInstance>()
                    {
                        new() {Amount = 2, FoodType = _slicedBread},
                        new() {Amount = 120, FoodType = _slicedChicken}
                    },
                    Outputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _chickenSandwich } },
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
                new BetterRecipe()
                {
                    RecipeId = 6,
                    MainOutput = _bread,
                    Inputs = new List<FoodInstance>()
                    {
                        new() {Amount = 120, FoodType = _eggs},
                        new() {Amount = 120, FoodType = _milk},
                        new() {Amount = 120, FoodType = _flour},
                    },
                    Outputs = new List<FoodInstance>() { new() { Amount = 1, FoodType = _bread } },
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

        public IList<BetterRecipe> GetRecipes()
        {
            return _recipes;
        }
    }
}