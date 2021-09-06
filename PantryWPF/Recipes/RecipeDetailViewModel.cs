﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Pantry.Core.Models;
using PantryWPF.Annotations;

namespace PantryWPF.Recipes
{
    public class RecipeDetailViewModel : BetterRecipe, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FakeSaveCommand FakeSaveCommand { get; set; }
        public RecipeDetailViewModel()
        {
            FakeSaveCommand = new FakeSaveCommand(this);
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FakeSaveCommand : ICommand
    {
        private readonly RecipeDetailViewModel _recipeDetailViewModel;

        public FakeSaveCommand(RecipeDetailViewModel recipeDetailViewModel)
        {
            _recipeDetailViewModel = recipeDetailViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var something = string.Join(Environment.NewLine, _recipeDetailViewModel.Inputs.Select(x => x.FoodType.Name + ": " + x.Amount));
            MessageBox.Show(something);
        }

        public event EventHandler CanExecuteChanged;
    }


}
