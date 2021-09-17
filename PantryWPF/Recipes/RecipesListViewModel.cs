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
                    Id = _selectedRecipe.Id,
                    RecipeFoods = _selectedRecipe.RecipeFoods,
                    MainOutput = _selectedRecipe.MainOutput,
                    //Outputs = _selectedRecipe.Outputs
                };
                OnPropertyChanged(nameof(SelectedRecipeDetailViewModel));
            }
        }

        public ObservableCollection<Recipe> ACollection { get; set; }

        public RecipesListViewModel()
        {
            //TODO: Replace this with constructor injection and Unity
            var rr = new HardCodedRecipeRepository();
            ACollection = new ObservableCollection<Recipe>(
                rr.GetRecipes());
            SelectedRecipeDetailViewModel = ACollection
                .Select(x => new RecipeDetailViewModel()
                {
                    RecipeSteps = x.RecipeSteps,
                    Id = x.Id,
                    RecipeFoods = x.RecipeFoods,
                    MainOutput = x.MainOutput,
                    //Outputs = x.Outputs
                }).First();
        }
    }
}
