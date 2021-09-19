using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Annotations;
using PantryWPF.Main;

namespace PantryWPF.Recipes
{
    public class RecipeDetailViewModel : Recipe, INotifyPropertyChanged
    {
        private readonly Recipe _selectedRecipe;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Pantry.Core.Models.Food> Foods { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public FakeSaveCommand FakeSaveCommand { get; set; }
        public string NewDescription { get; set; }
        public string NewDuration { get; set; }
        private readonly DataBase _dataBase;
        public ObservableCollection<RecipeStep> RecipeStepsList { get; set; }
        public RecipeDetailViewModel(Recipe selectedRecipe)
        {
            _dataBase = new DataBase();
            _selectedRecipe = _dataBase.Recipes.First(x => x.RecipeId == selectedRecipe.RecipeId); ;

            Foods = _dataBase.Foods.ToList();

            FakeSaveCommand = new FakeSaveCommand(this);
            SaveCommand = new DelegateCommand(SaveNewStep);

            LoadRecipeDetailData();
        }

        private void LoadRecipeDetailData()
        {
            LoadSteps();
        }

        private void LoadSteps()
        {
            var newList = _dataBase.RecipeSteps.Where(x => x.RecipeId == _selectedRecipe.RecipeId).ToList();
            if (RecipeStepsList is null)
            {
                RecipeStepsList = new ObservableCollection<RecipeStep>(newList);
            }
            else
            {
                RecipeStepsList.Clear();

                foreach (var x in newList)
                {
                    RecipeStepsList.Add(x);
                }
            }
            OnPropertyChanged(nameof(RecipeStepsList));
        }

        private void SaveNewStep()
        {
            bool GoodNumber = int.TryParse(NewDuration, out int tempDuration);

            if (!GoodNumber || string.IsNullOrWhiteSpace(NewDescription)) { return; }

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