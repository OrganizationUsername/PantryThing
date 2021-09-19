using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Annotations;
using PantryWPF.Main;

namespace PantryWPF.Recipes
{

    public static class HelperStuff
    {
        public static void ReplaceWith<T>(this ObservableCollection<T> a, IEnumerable<T> b)
        {
            if (a is null)
            {
                a = new ObservableCollection<T>(b);
            }
            else
            {
                a.Clear();
                foreach (var x in b)
                {
                    a.Add(x);
                }
            }
        }
    }

    public class RecipeDetailViewModel : Recipe, INotifyPropertyChanged
    {
        private readonly Recipe _selectedRecipe;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Pantry.Core.Models.Food> Foods { get; set; }
        public DelegateCommand SaveStepCommand { get; set; }
        public DelegateCommand SaveFoodCommand { get; set; }
        public DelegateCommand DeleteStepCommand { get; set; }
        public DelegateCommand DeleteFoodCommand { get; set; }

        public string NewDescription { get; set; }
        public string NewDuration { get; set; }
        public Pantry.Core.Models.Food NewFood { get; set; }
        public string NewFoodAmount { get; set; }

        public RecipeStep SelectedRecipeStep { get; set; }
        public RecipeFood SelectedRecipeFood { get; set; }
        private readonly DataBase _dataBase;
        public ObservableCollection<RecipeStep> RecipeStepsList { get; set; } = new ObservableCollection<RecipeStep>();
        public ObservableCollection<RecipeFood> RecipeFoodsList { get; set; } = new ObservableCollection<RecipeFood>();
        public RecipeDetailViewModel(Recipe selectedRecipe)
        {
            _dataBase = new DataBase();
            _selectedRecipe = _dataBase.Recipes.First(x => x.RecipeId == selectedRecipe.RecipeId); ;

            Foods = _dataBase.Foods.ToList();

            SaveStepCommand = new DelegateCommand(SaveNewStep);
            SaveFoodCommand = new DelegateCommand(SaveNewFood);
            DeleteStepCommand = new DelegateCommand(DeleteSelectedStep);
            DeleteFoodCommand = new DelegateCommand(DeleteSelectedFood);


            LoadRecipeDetailData();
        }

        private void DeleteSelectedFood()
        {
            if (SelectedRecipeFood is null) return;
            _dataBase.RecipeFoods.Remove(SelectedRecipeFood);
            _dataBase.SaveChanges();
            LoadRecipeDetailData();
        }

        private void DeleteSelectedStep()
        {
            if (SelectedRecipeStep is null) return;
            _dataBase.RecipeSteps.Remove(SelectedRecipeStep);
            _dataBase.SaveChanges();
            LoadRecipeDetailData();
        }

        private void SaveNewFood()
        {
            if (NewFood is not null && double.TryParse(NewFoodAmount, out double FoodAmount) && FoodAmount != 0)
            {
                var recipe = _dataBase.Recipes.Include(x => x.RecipeFoods).First(x => x.RecipeId == _selectedRecipe.RecipeId);
                var x = new RecipeFood() { Amount = FoodAmount, FoodId = NewFood.FoodId, RecipeId = _selectedRecipe.RecipeId };
                if (recipe.RecipeFoods is null)
                {
                    _dataBase.RecipeFoods.Add(x);
                    //recipe.RecipeFoods = new List<RecipeFood>() { x };
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

        private void LoadRecipeDetailData()
        {
            LoadSteps();
            LoadFoodInstances();
        }

        private void LoadFoodInstances()
        {
            if (_dataBase.RecipeFoods is null)
            {
                RecipeFoodsList = new ObservableCollection<RecipeFood>();
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
            var newList = _dataBase.RecipeSteps.Where(x => x.RecipeId == _selectedRecipe.RecipeId).ToList();

            RecipeStepsList.Clear();

            foreach (var x in newList)
            {
                RecipeStepsList.Add(x);
            }

            OnPropertyChanged(nameof(RecipeStepsList));
        }

        private void SaveNewStep()
        {
            bool goodNumber = int.TryParse(NewDuration, out int tempDuration);

            if (!goodNumber || string.IsNullOrWhiteSpace(NewDescription)) { return; }

            _dataBase.RecipeSteps.Add(new RecipeStep() { Instruction = NewDescription, RecipeId = _selectedRecipe.RecipeId, TimeCost = tempDuration });
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}