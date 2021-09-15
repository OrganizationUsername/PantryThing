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

        private BetterRecipe _selectedBetterRecipe;
        public BetterRecipe SelectedBetterRecipe
        {
            get => _selectedBetterRecipe;
            set
            {
                _selectedBetterRecipe = value;
                _selectedRecipeDetailViewModel = new RecipeDetailViewModel()
                {
                    RecipeSteps = _selectedBetterRecipe.RecipeSteps,
                    Id = _selectedBetterRecipe.Id,
                    Inputs = _selectedBetterRecipe.Inputs,
                    MainOutput = _selectedBetterRecipe.MainOutput,
                    Outputs = _selectedBetterRecipe.Outputs
                };
                OnPropertyChanged(nameof(SelectedRecipeDetailViewModel));
            }
        }

        public ObservableCollection<BetterRecipe> ACollection { get; set; }

        public RecipesListViewModel()
        {
            //TODO: Replace this with constructor injection and Unity
            var rr = new HardCodedRecipeRepository();
            ACollection = new ObservableCollection<BetterRecipe>(
                rr.GetRecipes());
            SelectedRecipeDetailViewModel = ACollection
                .Select(x => new RecipeDetailViewModel()
                {
                    RecipeSteps = x.RecipeSteps,
                    Id = x.Id,
                    Inputs = x.Inputs,
                    MainOutput = x.MainOutput,
                    Outputs = x.Outputs
                }).First();
        }
    }
}
