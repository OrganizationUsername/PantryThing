using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantry.Core.FoodProcessing;
using Pantry.Core.Models;
using Pantry.ServiceGateways;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Recipe
{
    public class RecipeDetailViewModel : Screen
    {
        private readonly ItemService _itemService;
        private Pantry.Core.Models.Recipe _selectedRecipe;

        public List<Core.Models.Food> Foods { get; set; }

        public DelegateCommand SaveStepCommand { get; set; }
        public DelegateCommand SaveFoodCommand { get; set; }
        public DelegateCommand DeleteStepCommand { get; set; }
        public DelegateCommand DeleteFoodCommand { get; set; }
        public DelegateCommand DeleteThisRecipeCommand { get; set; }
        public DelegateCommand CookCommand { get; set; }

        public BindableCollection<RecipeStep> RecipeStepsList { get; set; } = new();
        public BindableCollection<RecipeFood> RecipeFoodsList { get; set; } = new();
        public BindableCollection<EquipmentProjection> Equipments { get; set; }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetAndNotify(ref _description, value, nameof(Description));
        }
        private string _newDescription;
        public string NewDescription
        {
            get => _newDescription;
            set => SetAndNotify(ref _newDescription, value, nameof(NewDescription));
        }
        private string _newDuration;
        public string NewDuration
        {
            get => _newDuration;
            set => SetAndNotify(ref _newDuration, value, nameof(NewDuration));
        }
        private Core.Models.Food _newFood;
        public Core.Models.Food NewFood
        {
            get => _newFood;
            set => SetAndNotify(ref _newFood, value, nameof(NewFood));
        }
        private string _newFoodAmount;
        public string NewFoodAmount
        {
            get => _newFoodAmount;
            set => SetAndNotify(ref _newFoodAmount, value, nameof(NewFoodAmount));
        }
        private RecipeStep _selectedRecipeStep;
        public RecipeStep SelectedRecipeStep
        {
            get => _selectedRecipeStep;
            set => SetAndNotify(ref _selectedRecipeStep, value, nameof(SelectedRecipeStep));
        }
        private RecipeFood _selectedRecipeFood;
        public RecipeFood SelectedRecipeFood
        {
            get => _selectedRecipeFood;
            set => SetAndNotify(ref _selectedRecipeFood, value, nameof(SelectedRecipeFood));
        }
        private string _itemsUsed;
        public string ItemsUsed
        {
            get => _itemsUsed;
            set => SetAndNotify(ref _itemsUsed, value, nameof(ItemsUsed));
        }
        private bool _canCook;
        public bool CanCook
        {
            get => _canCook;
            set => SetAndNotify(ref _canCook, value, nameof(CanCook));
        }

        public RecipeDetailViewModel(ItemService itemService)
        {
            _itemService = itemService;
            SaveStepCommand = new(SaveNewStep);
            SaveFoodCommand = new(SaveNewFood);
            DeleteStepCommand = new(DeleteSelectedStep);
            DeleteFoodCommand = new(DeleteSelectedFood);
            DeleteThisRecipeCommand = new(DeleteThisRecipe);
            CookCommand = new(CookIt);
        }

        public void Load(int recipeId, string description)
        {
            Description = description;
            _selectedRecipe = _itemService.GetRecipe(recipeId).FirstOrDefault(x => x.RecipeId == recipeId);
            Foods = _itemService.GetFoods();
            Equipments = new(_itemService.GetEquipmentProjections());
            LoadRecipeDetailData();
            CanCook = CalculateCanCook();
        }

        public void CookIt()
        {
            return;
            BetterFoodProcessor foodProcessor = new();

            var collection = _itemService.GetLocationFoods()
                .Select(x => new RecipeFood() { Amount = x.Quantity, Food = x.Item.Food }).ToList();
            var canCook = foodProcessor.GetCookPlan(collection, _selectedRecipe, _itemService.GetRecipes().ToList());
            var inputsToBeConsumed = GetRelevantInventoryItems(canCook.TotalInput);
            foreach (var x in inputsToBeConsumed)
            {
                var y = _itemService.GetLocationFood(x.LocationFoodsId).FirstOrDefault();
                if (y is null)
                {
                    throw new("Lol, something went wrong.");
                }
                y.Quantity -= x.Quantity;
                if (y.Quantity == 0)
                {
                    y.Exists = false;
                }
            }

            var upc = canCook.TotalOutput.OrderBy(x => x.Amount).First().Food.FoodName;
            Core.Models.Item itemToUse = _itemService.GetItem(upc);
            if (itemToUse == null)
            {
                itemToUse = _itemService.AddSomething(canCook);
            }
            _itemService.AddNewLocationFood(canCook, itemToUse);

            CanCook = CalculateCanCook();
            LoadRecipeDetailData();
        }

        public bool CalculateCanCook()
        {
            BetterFoodProcessor foodProcessor = new();
            var currentFoodInventory = _itemService.GetLocationFoods()
                .Select(x => new RecipeFood() { Amount = x.Quantity, Food = x.Item.Food }).ToList();
            Trace.WriteLine("-----");
            Trace.WriteLine($"Trying to cook: {string.Join(Environment.NewLine, _selectedRecipe.RecipeFoods.Where(x => x.Amount < 0).Select(x => $"{x.Food.FoodName}: {x.Amount}"))}");
            Trace.WriteLine($"Recipe Requirements: {string.Join(Environment.NewLine, _selectedRecipe.RecipeFoods.Where(x => x.Amount > 0).Select(x => $"{x.Food.FoodName}: {x.Amount}"))}");
            Trace.WriteLine($"Ingredients:{string.Join(Environment.NewLine, _selectedRecipe.RecipeFoods.Select(x => $"{x.Food.FoodName}: {x.Amount}"))}");
            Trace.WriteLine("Current inventory:");
            Trace.WriteLine(string.Join(Environment.NewLine, currentFoodInventory.Select(x => $"{x.Food.FoodName}: {x.Amount}")));
            var canCook = foodProcessor.GetCookPlan(currentFoodInventory, _selectedRecipe, _itemService.GetRecipes());

            if (canCook.CanMake)
            {
                Trace.WriteLine("-----");
                Trace.WriteLine(string.Join(Environment.NewLine, canCook.TotalInput.Select(x => $"{x.Food.FoodName}, {x.Amount}")));
                ItemsUsed = string.Join(Environment.NewLine, GetRelevantInventoryItems(canCook.TotalInput).Select(x => $"{x.Item.Food.FoodName}- {x.LocationFoodsId}: {x.Quantity}"));
            }
            else { ItemsUsed = ""; }

            return canCook.CanMake;
        }

        private List<LocationFoods> GetRelevantInventoryItems(IEnumerable<RecipeFood> recipeFoods)
        {
            List<LocationFoods> outputFoods = new();
            var locationFoods = _itemService.GetLocationFoods()
                .Select(x => new LocationFoods() { Item = x.Item, Quantity = x.Quantity }).ToList();
            foreach (var x in recipeFoods)
            {
                var totalAmount = x.Amount;
                while (totalAmount > 0)
                {
                    var locationFood = locationFoods.FirstOrDefault(y => y.Quantity > 0 && y.Item.FoodId == x.Food.FoodId);
                    if (locationFood is null) { throw new("Thought we could make it, but we cannot."); }

                    var amountToRemove = Math.Min(totalAmount, locationFood.Quantity);
                    locationFood.Quantity -= amountToRemove;
                    totalAmount -= amountToRemove;

                    outputFoods.Add(new() { Item = locationFood.Item, Quantity = amountToRemove, LocationFoodsId = locationFood.LocationFoodsId });
                }
            }
            return outputFoods;
        }

        private void LoadRecipeDetailData()
        {
            LoadSteps();
            LoadFoodInstances();
        }

        private void DeleteThisRecipe()
        {
            var thisRecipe = _itemService.GetRecipe(_selectedRecipe.RecipeId).FirstOrDefault();
            if (thisRecipe is null || _selectedRecipe is null)
            {
                return;
            }

            _itemService.DeleteRecipe(thisRecipe);
            LoadRecipeDetailData();
        }

        private void DeleteSelectedFood()
        {
            if (SelectedRecipeFood is null || _selectedRecipe is null) return;
            _itemService.DeleteFood(SelectedRecipeFood);
            LoadRecipeDetailData();
        }

        private void DeleteSelectedStep()
        {
            if (SelectedRecipeStep is null || _selectedRecipe is null) return;
            _itemService.DeleteRecipeStep(SelectedRecipeStep);
            LoadRecipeDetailData();
        }

        private void SaveNewFood()
        {
            if (_selectedRecipe is null) { return; }
            if (NewFood is not null && double.TryParse(NewFoodAmount, out var foodAmount) && foodAmount != 0)
            {
                _itemService.AddRecipeFood(_selectedRecipe.RecipeId, NewFood.FoodId, foodAmount);
                NewFoodAmount = "";
                LoadRecipeDetailData();
            }
        }

        private void LoadFoodInstances()
        {
            RecipeFoodsList.Clear();
            RecipeFoodsList.AddRange(_itemService.GetRecipeFoods(_selectedRecipe));
        }

        private void LoadSteps()
        {
            RecipeStepsList.Clear();
            RecipeStepsList.AddRange(_itemService.GetRecipeSteps(_selectedRecipe));
        }

        private void SaveNewStep()
        {
            if (_selectedRecipe is null) return;
            //ToDo: Why do I not yet have centralized exception handling?

            var goodNumber = int.TryParse(NewDuration, out var tempDuration);

            if (!goodNumber || string.IsNullOrWhiteSpace(NewDescription)) return;

            _itemService.AddRecipeStep(NewDescription, _selectedRecipe.RecipeId, tempDuration);

            NewDescription = "";
            NewDuration = "";
            LoadRecipeDetailData();
        }
    }
}