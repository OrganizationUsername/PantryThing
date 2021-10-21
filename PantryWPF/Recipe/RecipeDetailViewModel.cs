using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantry.Core.FoodProcessing;
using Pantry.Core.Models;
using PantryWPF.Annotations;
using PantryWPF.Main;
using ServiceGateways;

namespace Pantry.WPF.Recipe
{
    public class RecipeDetailViewModel : Pantry.Core.Models.Recipe, INotifyPropertyChanged
    {
        private readonly Pantry.Core.Models.Recipe _selectedRecipe;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Food> Foods { get; set; }
        public DelegateCommand SaveStepCommand { get; set; }
        public DelegateCommand SaveFoodCommand { get; set; }
        public DelegateCommand DeleteStepCommand { get; set; }
        public DelegateCommand DeleteFoodCommand { get; set; }
        public DelegateCommand DeleteThisRecipeCommand { get; set; }
        public string NewDescription { get; set; }
        public DelegateCommand CookCommand { get; set; }
        public string NewDuration { get; set; }
        public Food NewFood { get; set; }
        public string NewFoodAmount { get; set; }
        public RecipeStep SelectedRecipeStep { get; set; }
        public RecipeFood SelectedRecipeFood { get; set; }
        public ObservableCollection<RecipeStep> RecipeStepsList { get; set; } = new();
        public ObservableCollection<RecipeFood> RecipeFoodsList { get; set; } = new();
        public ObservableCollection<EquipmentProjection> Equipments { get; set; }
        private ItemService _itemService;
        public string ItemsUsed { get; set; } = "";
        public bool CanCook { get; set; }

        public RecipeDetailViewModel(Pantry.Core.Models.Recipe selectedRecipe)
        {
            _itemService = new();
            SaveStepCommand = new(SaveNewStep);
            SaveFoodCommand = new(SaveNewFood);
            DeleteStepCommand = new(DeleteSelectedStep);
            DeleteFoodCommand = new(DeleteSelectedFood);
            DeleteThisRecipeCommand = new(DeleteThisRecipe);
            CookCommand = new(CookIt);

            _selectedRecipe = _itemService.GetRecipe(selectedRecipe.RecipeId).FirstOrDefault(x => x.RecipeId == selectedRecipe.RecipeId);
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
            Item itemToUse = _itemService.GetItem(upc);
            if (itemToUse == null)
            {
                itemToUse = _itemService.AddSomething(canCook);
            }
            _itemService.AddNewLocationFood(canCook, itemToUse);

            CanCook = CalculateCanCook();
            LoadRecipeDetailData();
            OnPropertyChanged(nameof(canCook));
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
            OnPropertyChanged(nameof(ItemsUsed));

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
                var recipe = _itemService.GetRecipes().First(x => x.RecipeId == _selectedRecipe.RecipeId);
                var x = new RecipeFood() { Amount = foodAmount, FoodId = NewFood.FoodId, RecipeId = _selectedRecipe.RecipeId };
                if (recipe.RecipeFoods is null)
                {
                    //_dataBase.RecipeFoods.Add(x); //ToDo: Fix whatever this is
                }
                else
                {
                    recipe.RecipeFoods.Add(x);
                }
                //_dataBase.SaveChanges(); //ToDo: Fix whatever this is
                NewFoodAmount = "";
                OnPropertyChanged(nameof(NewFoodAmount));
                LoadRecipeDetailData();
            }
        }

        private void LoadFoodInstances()
        {
            var newList = _itemService.GetRecipeFoods(_selectedRecipe);
            RecipeFoodsList.Clear();

            foreach (var x in newList)
            {
                RecipeFoodsList.Add(x);
            }

            OnPropertyChanged(nameof(RecipeFoodsList));
        }

        private void LoadSteps()
        {
            List<RecipeStep> newList = _itemService.GetRecipeSteps(_selectedRecipe);

            RecipeStepsList.Clear();

            foreach (var x in newList)
            {
                RecipeStepsList.Add(x);
            }

            OnPropertyChanged(nameof(RecipeStepsList));
        }

        private void SaveNewStep()
        {
            if (_selectedRecipe is null) return;
            //ToDo: Why do I not yet have centralized exception handling?

            var goodNumber = int.TryParse(NewDuration, out var tempDuration);

            if (!goodNumber || string.IsNullOrWhiteSpace(NewDescription)) return;

            var y = new RecipeStepEquipment() { EquipmentId = 1 };

            _itemService.AddRecipeStep(NewDescription, _selectedRecipe.RecipeId, tempDuration);

            NewDescription = "";
            NewDuration = "";
            OnPropertyChanged(nameof(NewDescription));
            OnPropertyChanged(nameof(NewDuration));
            LoadRecipeDetailData();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}