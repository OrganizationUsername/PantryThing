using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PantryWPF.Recipes
{
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
            var something = string.Join(Environment.NewLine, _recipeDetailViewModel.RecipeFoods.Select(x => x.Food.FoodName + ": " + x.Amount));
            MessageBox.Show(something);
        }

        public event EventHandler CanExecuteChanged;
    }
}