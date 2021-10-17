using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.FoodProcessing;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Annotations;
using PantryWPF.Main;

namespace PantryWPF.Recipe
{
    public class RecipeDetailViewModel : Pantry.Core.Models.Recipe, INotifyPropertyChanged
    {
        private readonly Pantry.Core.Models.Recipe _selectedRecipe;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Pantry.Core.Models.Food> Foods { get; set; }
        public DelegateCommand SaveStepCommand { get; set; }
        public DelegateCommand SaveFoodCommand { get; set; }
        public DelegateCommand DeleteStepCommand { get; set; }
        public DelegateCommand DeleteFoodCommand { get; set; }
        public DelegateCommand DeleteThisRecipeCommand { get; set; }
        public string NewDescription { get; set; }
        public DelegateCommand CookCommand { get; set; }

        public string NewDuration { get; set; }
        public Pantry.Core.Models.Food NewFood { get; set; }
        public string NewFoodAmount { get; set; }

        public RecipeStep SelectedRecipeStep { get; set; }
        public RecipeFood SelectedRecipeFood { get; set; }
        private readonly DataBase _dataBase;
        public ObservableCollection<RecipeStep> RecipeStepsList { get; set; } = new();
        public ObservableCollection<RecipeFood> RecipeFoodsList { get; set; } = new();
        public string ItemsUsed { get; set; } = "";
        public bool CanCook { get; set; }

        public RecipeDetailViewModel(Pantry.Core.Models.Recipe selectedRecipe)
        {
            SaveStepCommand = new(SaveNewStep);
            SaveFoodCommand = new(SaveNewFood);
            DeleteStepCommand = new(DeleteSelectedStep);
            DeleteFoodCommand = new(DeleteSelectedFood);
            DeleteThisRecipeCommand = new(DeleteThisRecipe);
            CookCommand = new(CookIt);

            _dataBase = new();
            _selectedRecipe = _dataBase.Recipes.FirstOrDefault(x => x.RecipeId == selectedRecipe.RecipeId);
            Foods = _dataBase.Foods.ToList();
            LoadRecipeDetailData();
            CanCook = CalculateCanCook();
        }

        public void CookIt()
        {
            BetterFoodProcessor foodProcessor = new();

            var collection = _dataBase.LocationFoods
                .Include(x => x.Item)
                .ThenInclude(x => x.Food)
                .ToList()
                .Select(x => new RecipeFood() { Amount = x.Quantity, Food = x.Item.Food }).ToList();
            var canCook = foodProcessor.GetCookPlan(collection, _selectedRecipe, _dataBase.Recipes.ToList());
            var inputsToBeConsumed = GetRelevantInventoryItems(canCook.TotalInput);
            foreach (var x in inputsToBeConsumed)
            {
                var y = _dataBase.LocationFoods.First(z => z.Quantity > 0 && z.LocationFoodsId == x.LocationFoodsId);
                y.Quantity -= x.Quantity;
                if (y.Quantity == 0)
                {
                    y.Exists = false;
                }
            }

            var upc = canCook.TotalOutput.OrderByDescending(x => x.Amount).First().Food.FoodName;
            var itemToUse = _dataBase.Items.FirstOrDefault(x => x.Upc == upc);

            if (itemToUse is null)
            {
                itemToUse = _dataBase.Items.Add(new()
                {
                    FoodId = canCook.TotalOutput.OrderByDescending(x => x.Amount).First().Food.FoodId,
                    Unit = null,
                    Upc = upc,
                    Weight = canCook.TotalOutput.First().Amount
                }).Entity;
            }

            _dataBase.LocationFoods.Add(new()
            {
                Exists = true,
                ExpiryDate = DateTime.Now,
                Quantity = canCook.TotalOutput.First().Amount,
                Location = _dataBase.Locations.First(),
                OpenDate = DateTime.MinValue,
                PurchaseDate = DateTime.MinValue,
                Item = itemToUse
            });
            _dataBase.SaveChanges();
            CanCook = CalculateCanCook();
            LoadRecipeDetailData();
            OnPropertyChanged(nameof(canCook));
        }

        public bool CalculateCanCook()
        {
            BetterFoodProcessor foodProcessor = new();
            var currentFoodInventory = _dataBase.LocationFoods
                .Include(x => x.Item)
                .ThenInclude(x => x.Food)
                .ToList()
                .Select(x => new RecipeFood() { Amount = x.Quantity, Food = x.Item.Food }).ToList();
            Trace.WriteLine("-----");
            Trace.WriteLine($"Trying to cook: {string.Join(Environment.NewLine, _selectedRecipe.RecipeFoods.Where(x => x.Amount < 0).Select(x => $"{x.Food.FoodName}: {x.Amount}"))}");
            Trace.WriteLine($"Recipe Requirements: {string.Join(Environment.NewLine, _selectedRecipe.RecipeFoods.Where(x => x.Amount > 0).Select(x => $"{x.Food.FoodName}: {x.Amount}"))}");
            Trace.WriteLine($"Ingredients:{string.Join(Environment.NewLine, _selectedRecipe.RecipeFoods.Select(x => $"{x.Food.FoodName}: {x.Amount}"))}");
            Trace.WriteLine("Current inventory:");
            Trace.WriteLine(string.Join(Environment.NewLine, currentFoodInventory.Select(x => $"{x.Food.FoodName}: {x.Amount}")));
            var canCook = foodProcessor.GetCookPlan(currentFoodInventory, _selectedRecipe, _dataBase.Recipes.Include(x => x.RecipeFoods).ToList());

            if (canCook.CanMake)
            {
                Trace.WriteLine("-----");
                Trace.WriteLine(string.Join(Environment.NewLine, canCook.TotalInput.Select(x => $"{x.Food.FoodName}, {x.Amount}")));
                ItemsUsed = string.Join(Environment.NewLine, GetRelevantInventoryItems(canCook.TotalInput).Select(x => $"{x.Item.Food.FoodName}- {x.LocationFoodsId}: {x.Quantity}"));

            }
            else
            {
                ItemsUsed = "";
            }
            OnPropertyChanged(nameof(ItemsUsed));
            return canCook.CanMake;
        }

        private List<LocationFoods> GetRelevantInventoryItems(IEnumerable<RecipeFood> recipeFoods)
        {
            List<LocationFoods> outputFoods = new();
            var locationFoods = _dataBase.LocationFoods
                .AsNoTracking()
                .Include(x => x.Item).ThenInclude(x => x.Food)
                .Where(x => x.Exists && x.Quantity > 0)
                .OrderBy(x => x.Quantity).ToList();
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
            var thisRecipe = _dataBase.Recipes.FirstOrDefault(x => x.RecipeId == _selectedRecipe.RecipeId);
            if (thisRecipe is null || _selectedRecipe is null)
            {
                return;
            }
            _dataBase.Recipes.Remove(thisRecipe);
            _dataBase.SaveChanges();
            LoadRecipeDetailData();
        }

        private void DeleteSelectedFood()
        {
            if (SelectedRecipeFood is null || _selectedRecipe is null) return;
            _dataBase.RecipeFoods.Remove(SelectedRecipeFood);
            _dataBase.SaveChanges();
            LoadRecipeDetailData();
        }

        private void DeleteSelectedStep()
        {
            if (SelectedRecipeStep is null || _selectedRecipe is null) return;
            _dataBase.RecipeSteps.Remove(SelectedRecipeStep);
            _dataBase.SaveChanges();
            LoadRecipeDetailData();
        }

        private void SaveNewFood()
        {
            if (_selectedRecipe is null) { return; }
            if (NewFood is not null && double.TryParse(NewFoodAmount, out var foodAmount) && foodAmount != 0)
            {
                var recipe = _dataBase.Recipes.Include(x => x.RecipeFoods).First(x => x.RecipeId == _selectedRecipe.RecipeId);
                var x = new RecipeFood() { Amount = foodAmount, FoodId = NewFood.FoodId, RecipeId = _selectedRecipe.RecipeId };
                if (recipe.RecipeFoods is null)
                {
                    _dataBase.RecipeFoods.Add(x);
                }
                else
                {
                    recipe.RecipeFoods.Add(x);
                }
                _dataBase.SaveChanges();
                NewFoodAmount = "";
                OnPropertyChanged(nameof(NewFoodAmount));
                LoadRecipeDetailData();
            }
        }

        private void LoadFoodInstances()
        {
            if (_selectedRecipe is null) { return; }

            if (_dataBase.RecipeFoods is null)
            {
                RecipeFoodsList = new();
                return;
            }
            var newList = _dataBase.RecipeFoods.Where(x => x.RecipeId == _selectedRecipe.RecipeId).ToList();
            RecipeFoodsList.Clear();

            foreach (var x in newList)
            {
                RecipeFoodsList.Add(x);
            }
            OnPropertyChanged(nameof(RecipeFoodsList));
        }

        private void LoadSteps()
        {
            List<RecipeStep> newList;
            if (_selectedRecipe is null && (!_dataBase.RecipeSteps.Any() ||
                                        !_dataBase.RecipeSteps.Any(x => x.RecipeId == _selectedRecipe.RecipeId)))
            {
                newList = new();
            }
            else
            {
                newList = _dataBase.RecipeSteps.Where(x => x.RecipeId == _selectedRecipe.RecipeId)
                    .Include(y => y.RecipeStepEquipment)
                    .Include(y => y.RecipeStepEquipment)
                    .ThenInclude(y => y.Equipment).ToList();
            }



            RecipeStepsList.Clear();

            foreach (var x in newList)
            {
                RecipeStepsList.Add(x);
            }

            OnPropertyChanged(nameof(RecipeStepsList));
        }

        private void SaveNewStep()
        {
            if (_selectedRecipe is null) { return; }

            var goodNumber = int.TryParse(NewDuration, out var tempDuration);

            if (!goodNumber || string.IsNullOrWhiteSpace(NewDescription)) { return; }

            _dataBase.RecipeSteps.Add(new() { Instruction = NewDescription, RecipeId = _selectedRecipe.RecipeId, TimeCost = tempDuration });
            _dataBase.SaveChanges();
            NewDescription = "";
            NewDuration = "";
            Trace.WriteLine(_dataBase.RecipeSteps.Count());
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