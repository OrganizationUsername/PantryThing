using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Recipes
{
    public class RecipesListViewModel : VmBase
    {
        private DataBase _dataBase;
        private RecipeDetailViewModel _selectedRecipeDetailViewModel;
        public DelegateCommand AddRecipeCommand { get; set; }
        public ObservableCollection<Recipe> ACollection { get; set; }
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

        private Recipe _selectedRecipe;

        public Recipe SelectedRecipe
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
            _dataBase = new();
            LoadData();
            AddRecipeCommand = new(AddRecipe);
        }


        public void LoadData()
        {
            if (ACollection is null)
            {
                ACollection = new(_dataBase.Recipes.ToList());
            }
            else
            {
                ACollection.Clear();
                foreach (var x in _dataBase.Recipes)
                {
                    ACollection.Add(x);
                }
            }

            OnPropertyChanged(nameof(ACollection));
        }

        public void AddRecipe()
        {
            if (string.IsNullOrWhiteSpace(NewRecipeName) || _dataBase.Recipes.Any(x => x.Description == NewRecipeName)) { return; }
            _dataBase.Recipes.Add(new() { Description = NewRecipeName });
            _dataBase.SaveChanges();
            _dataBase = new(); //otherwise recipe is always null if it's the first Recipe to be added.

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
