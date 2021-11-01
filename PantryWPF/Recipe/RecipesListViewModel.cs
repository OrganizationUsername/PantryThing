using System;
using System.Linq;
using Pantry.ServiceGateways.Recipe;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Recipe
{
    public class RecipesListViewModel : Screen
    {
        private readonly RecipeServiceGateway _recipeServiceGateway;
        private readonly Func<RecipeDetailViewModel> _recipeDetailFactory;

        public DelegateCommand AddRecipeCommand { get; set; }
        public BindableCollection<Pantry.Core.Models.Recipe> ACollection { get; set; } = new();

        private string _newRecipeName;
        public string NewRecipeName
        {
            get => _newRecipeName;
            set => SetAndNotify(ref _newRecipeName, value, nameof(NewRecipeName));
        }

        private RecipeDetailViewModel _selectedRecipeDetailViewModel;
        public RecipeDetailViewModel SelectedRecipeDetailViewModel
        {
            get => _selectedRecipeDetailViewModel;
            set => SetAndNotify(ref _selectedRecipeDetailViewModel, value, nameof(SelectedRecipeDetailViewModel));
        }

        private Pantry.Core.Models.Recipe _selectedRecipe;

        public Pantry.Core.Models.Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (SetAndNotify(ref _selectedRecipe, value, nameof(SelectedRecipe)))
                {
                    SelectedRecipeDetailViewModel = _selectedRecipe is not null ? _recipeDetailFactory() : null;
                    if (_selectedRecipe != null)
                    {
                        SelectedRecipeDetailViewModel?.Load(_selectedRecipe.RecipeId, _selectedRecipe.Description);
                    }
                }
            }
        }

        public RecipesListViewModel(RecipeServiceGateway recipeServiceGateway, Func<RecipeDetailViewModel> recipeDetailFactory)
        {
            _recipeDetailFactory = recipeDetailFactory;
            _recipeServiceGateway = recipeServiceGateway;
            AddRecipeCommand = new(AddRecipe);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadData();
        }

        public void LoadData()
        {
            ACollection.Clear();
            ACollection.AddRange(_recipeServiceGateway.GetRecipes());
        }

        public void AddRecipe()
        {
            if (string.IsNullOrWhiteSpace(NewRecipeName) || _recipeServiceGateway.GetRecipes().Any(x => x.Description == NewRecipeName)) { return; }

            _recipeServiceGateway.AddEmptyRecipe(NewRecipeName);

            LoadData();
            var newRecipe = ACollection.FirstOrDefault(x => x.Description == NewRecipeName);

            if (newRecipe is null)
            {
                return;
            }

            LoadData();
            SelectedRecipe = ACollection.Last();
            NewRecipeName = "";
        }
    }
}
