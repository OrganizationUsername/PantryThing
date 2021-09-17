//using Pantry.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Pantry.Data
//{

//    public class HardCodedFoodRepository
//    {
//        private readonly Food _frozenChicken = new() { FoodId = 0, FoodName = "Frozen Chicken", BetterRecipes = null };
//        private readonly Food _rawChicken = new() { FoodId = 1, FoodName = "Raw Chicken", };
//        private readonly Food _bbqSauce = new() { FoodId = 2, FoodName = "BBQ Sauce", BetterRecipes = null };
//        private readonly Food _cookedChicken = new() { FoodId = 3, FoodName = "Cooked Chicken" };
//        private readonly Food _slicedChicken = new() { FoodId = 4, FoodName = "Sliced Chicken" };
//        private readonly Food _flour = new() { FoodId = 5, FoodName = "Flour", BetterRecipes = null };
//        private readonly Food _eggs = new() { FoodId = 6, FoodName = "Eggs", BetterRecipes = null };
//        private readonly Food _milk = new() { FoodId = 7, FoodName = "Milk", BetterRecipes = null };
//        private readonly Food _bread = new() { FoodId = 8, FoodName = "Bread" };
//        private readonly Food _slicedBread = new() { FoodId = 9, FoodName = "Sliced Bread" };
//        private readonly Food _chickenSandwich = new() { FoodId = 10, FoodName = "Chicken Sandwich" };
//        private List<Food> Foods { get; set; } = new List<Food>();

//        public HardCodedFoodRepository()
//        {
//            Foods.Add(_frozenChicken);
//            Foods.Add(_rawChicken);
//            Foods.Add(_bbqSauce);
//            Foods.Add(_cookedChicken);
//            Foods.Add(_slicedChicken);
//            Foods.Add(_flour);
//            Foods.Add(_eggs);
//            Foods.Add(_milk);
//            Foods.Add(_bread);
//            Foods.Add(_slicedBread);
//            Foods.Add(_chickenSandwich);
//        }
//        public IList<Food> GetFoods()
//        {
//            return Foods;
//        }
//    }

//    public class HardCodedRecipeRepository
//    {
//        private readonly List<Recipe> _recipes = new();
//        private readonly Equipment _breadMachine = new()
//        { FoodName = "Bread Machine", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
//        private readonly Equipment _humanMachine = new()
//        { FoodName = "Human", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
//        private readonly Equipment _fridge = new()
//        { FoodName = "Fridge", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
//        private readonly Equipment _sousVide = new()
//        { FoodName = "Sous Vide", BookedTimes = new List<(DateTime startTime, DateTime endTime, string TaskName)>() };
//        private readonly Food _frozenChicken = new() { FoodId = 0, FoodName = "Frozen Chicken", BetterRecipes = null };
//        private readonly Food _rawChicken = new() { FoodId = 1, FoodName = "Raw Chicken", };
//        private readonly Food _bbqSauce = new() { FoodId = 2, FoodName = "BBQ Sauce", BetterRecipes = null };
//        private readonly Food _cookedChicken = new() { FoodId = 3, FoodName = "Cooked Chicken" };
//        private readonly Food _slicedChicken = new() { FoodId = 4, FoodName = "Sliced Chicken" };
//        private readonly Food _flour = new() { FoodId = 5, FoodName = "Flour", BetterRecipes = null };
//        private readonly Food _eggs = new() { FoodId = 6, FoodName = "Eggs", BetterRecipes = null };
//        private readonly Food _milk = new() { FoodId = 7, FoodName = "Milk", BetterRecipes = null };
//        private readonly Food _bread = new() { FoodId = 8, FoodName = "Bread" };
//        private readonly Food _slicedBread = new() { FoodId = 9, FoodName = "Sliced Bread" };
//        private readonly Food _chickenSandwich = new() { FoodId = 10, FoodName = "Chicken Sandwich" };

//        public HardCodedRecipeRepository()
//        {
//            _recipes.Add(
//             new Recipe()
//             {
//                 RecipeFoodId = 1,
//                 MainOutput = _cookedChicken,
//                 RecipeFoods = new List<RecipeFood>()
//                 {
//                        new() {Amount = 120, Food = _frozenChicken, RecipeFoodId = _frozenChicken.FoodId},
//                        new() {Amount = 1, Food = _bbqSauce , RecipeFoodId = _bbqSauce.FoodId },
//                        new() {Amount = -120, Food = _cookedChicken, RecipeFoodId = _cookedChicken.FoodId },
//                 },
//                 RecipeSteps = new List<RecipeStep>()
//                 {
//                        new()
//                        {
//                            Instruction = "Put chicken in Sous Vide.", TimeCost = 1,
//                            Equipments = new List<Equipment>() {_sousVide, _humanMachine}
//                        },
//                        new()
//                        {
//                            Instruction = "Let it cook.", TimeCost = 120, Equipments = new List<Equipment>() {_sousVide}
//                        },
//                        new()
//                        {
//                            Instruction = "Take chicken out.", TimeCost = 1,
//                            Equipments = new List<Equipment>() {_sousVide, _humanMachine}
//                        },
//                 }
//             });
//            _recipes.Add(
//                new Recipe()
//                {
//                    RecipeFoodId = 2,
//                    MainOutput = _rawChicken,
//                    RecipeFoods = new List<RecipeFood>()
//                    {
//                        new() { Amount = 120, Food = _frozenChicken, RecipeFoodId = _frozenChicken.FoodId },
//                        new() { Amount = -120, Food = _rawChicken, RecipeFoodId = _rawChicken.FoodId },
//                    },
//                    RecipeSteps = new List<RecipeStep>()
//                    {
//                        new()
//                        {
//                            Instruction = "Put chicken in fridge.", TimeCost = 1,
//                            Equipments = new List<Equipment>() {_fridge, _humanMachine}
//                        },
//                        new()
//                        {
//                            Instruction = "Let it defrost.", TimeCost = 1440,
//                            Equipments = new List<Equipment>() {_fridge}
//                        }
//                    }
//                });
//            _recipes.Add(
//                new Recipe()
//                {
//                    RecipeFoodId = 3,
//                    MainOutput = _slicedChicken,
//                    RecipeFoods = new List<RecipeFood>()
//                    {
//                        new() { Amount = 120, Food = _cookedChicken, RecipeFoodId = _cookedChicken.FoodId },
//                        new() { Amount = -120, Food = _slicedChicken, RecipeFoodId = _slicedChicken.FoodId },
//                    },
//                    RecipeSteps = new List<RecipeStep>()
//                    {
//                        new()
//                        {
//                            Instruction = "Cut chicken with a knife.", TimeCost = 3,
//                            Equipments = new List<Equipment>() {_humanMachine}
//                        },
//                    }
//                });
//            _recipes.Add(
//                new Recipe()
//                {
//                    RecipeFoodId = 4,
//                    MainOutput = _slicedBread,
//                    RecipeFoods = new List<RecipeFood>()
//                    {
//                        new() { Amount = 1, Food = _bread, RecipeFoodId = _bread.FoodId },
//                        new() { Amount = -10, Food = _slicedBread, RecipeFoodId = _slicedBread.FoodId },
//                    },
//                    RecipeSteps = new List<RecipeStep>()
//                    {
//                        new()
//                        {
//                            Instruction = "Cut Bread", TimeCost = 2, Equipments = new List<Equipment>() {_humanMachine}
//                        },
//                    }
//                });
//            _recipes.Add(
//                new Recipe()
//                {
//                    RecipeFoodId = 5,
//                    MainOutput = _chickenSandwich,
//                    RecipeFoods = new List<RecipeFood>()
//                    {
//                        new() {Amount = 2, Food = _slicedBread, RecipeFoodId = _slicedBread.FoodId},
//                        new() {Amount = 120, Food = _slicedChicken, RecipeFoodId = _slicedChicken.FoodId},
//                        new() {Amount = -1, Food = _chickenSandwich , RecipeFoodId = _chickenSandwich.FoodId},
//                    },
//                    RecipeSteps = new List<RecipeStep>()
//                    {
//                        new()
//                        {
//                            Instruction = "Assemble Sandwich", TimeCost = 1,
//                            Equipments = new List<Equipment>() {_humanMachine}
//                        },
//                    }
//                });
//            _recipes.Add(
//                new Recipe()
//                {
//                    RecipeFoodId = 6,
//                    MainOutput = _bread,
//                    RecipeFoods = new List<RecipeFood>()
//                    {
//                        new() {Amount = 120, Food = _eggs, RecipeFoodId = _eggs.FoodId},
//                        new() {Amount = 120, Food = _milk, RecipeFoodId = _milk.FoodId},
//                        new() {Amount = 120, Food = _flour, RecipeFoodId = _flour.FoodId},
//                        new() { Amount = -1, Food = _bread, RecipeFoodId = _bread.FoodId },
//                    },
//                    RecipeSteps = new List<RecipeStep>()
//                    {
//                        new()
//                        {
//                            Instruction = "Insert into Bread Machine.", TimeCost = 1,
//                            Equipments = new() {_humanMachine, _breadMachine}
//                        },
//                        new()
//                        {
//                            Instruction = "Bread Machine cooks.", TimeCost = 180, Equipments = new() {_breadMachine}
//                        },
//                        new()
//                        {
//                            Instruction = "Extract bread from bread machine.", TimeCost = 1,
//                            Equipments = new() {_humanMachine, _breadMachine}
//                        },
//                    }
//                });
//        }

//        public IList<Recipe> GetRecipes()
//        {
//            return _recipes;
//        }
//    }
//}
