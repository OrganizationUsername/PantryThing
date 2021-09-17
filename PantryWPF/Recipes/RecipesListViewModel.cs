using System.Collections.ObjectModel;
using System.Linq;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Recipes
{
    public class RecipesListViewModel : VmBase
    {
        private RecipeDetailViewModel _selectedRecipeDetailViewModel;
        public RecipeDetailViewModel SelectedRecipeDetailViewModel
        {
            get => _selectedRecipeDetailViewModel;
            set
            {
                _selectedRecipeDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        private Recipe _selectedRecipe;
        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                _selectedRecipe = value;
                _selectedRecipeDetailViewModel = new RecipeDetailViewModel()
                {
                    RecipeSteps = _selectedRecipe.RecipeSteps,
                    RecipeId = _selectedRecipe.RecipeId,
                    RecipeFoods = _selectedRecipe.RecipeFoods,
                    //Outputs = _selectedRecipe.Outputs
                };
                OnPropertyChanged(nameof(SelectedRecipeDetailViewModel));
            }
        }

        public ObservableCollection<Recipe> ACollection { get; set; }

        public RecipesListViewModel()
        {
   
        }
    }
}
