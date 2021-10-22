using System.Collections.ObjectModel;
using System.Linq;
using Pantry.ServiceGateways;
using Pantry.WPF.Shared;

namespace Pantry.WPF.Recipe
{
    public class RecipesListViewModel : VmBase
    {
        private RecipeDetailViewModel _selectedRecipeDetailViewModel;
        public DelegateCommand AddRecipeCommand { get; set; }
        public ObservableCollection<Pantry.Core.Models.Recipe> ACollection { get; set; }
        private readonly ItemService _itemService;
        public string NewRecipeName { get; set; }

        public RecipeDetailViewModel SelectedRecipeDetailViewModel
        {
            get => _selectedRecipeDetailViewModel;
            set
            {
                _selectedRecipeDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        private Pantry.Core.Models.Recipe _selectedRecipe;

        public Pantry.Core.Models.Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                _selectedRecipe = value;
                _selectedRecipeDetailViewModel = new(_selectedRecipe);
                OnPropertyChanged(nameof(SelectedRecipeDetailViewModel));
            }
        }

        public RecipesListViewModel() //ToDo: Figure out why this isn't called when navigated to.
        {
            _itemService = new();
            LoadData();
            AddRecipeCommand = new(AddRecipe);
        }


        public void LoadData()
        {
            if (ACollection is null)
            {
                ACollection = new(_itemService.GetRecipes());
            }
            else
            {
                ACollection.Clear();
                foreach (var x in _itemService.GetRecipes())
                {
                    ACollection.Add(x);
                }
            }

            OnPropertyChanged(nameof(ACollection));
        }

        public void AddRecipe()
        {
            if (string.IsNullOrWhiteSpace(NewRecipeName) || _itemService.GetRecipes().Any(x => x.Description == NewRecipeName)) { return; }

            _itemService.AddEmptyRecipe(NewRecipeName);

            LoadData();
            var newRecipe = ACollection.FirstOrDefault(x => x.Description == NewRecipeName);

            if (newRecipe is null)
            {
                return;
            }

            LoadData();
            SelectedRecipe = this.ACollection.Last();
            OnPropertyChanged(nameof(SelectedRecipe));

            NewRecipeName = "";
            OnPropertyChanged(nameof(NewRecipeName));
        }

    }
}
