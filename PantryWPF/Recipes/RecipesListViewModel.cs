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
                _selectedRecipeDetailViewModel = new RecipeDetailViewModel(_selectedRecipe);
                OnPropertyChanged(nameof(SelectedRecipeDetailViewModel));
            }
        }

        public RecipesListViewModel()
        {
            _dataBase = new DataBase();
            CleanDatabase();
            LoadRecipes();
            AddRecipeCommand = new DelegateCommand(AddRecipe);
        }

        public void CleanDatabase()
        {
            List<string> nameList = new();
            if (_dataBase.Recipes is null)
            {
                return;
            }
            for (var index = 0; index < _dataBase.Recipes.Count(); index++)
            {
                var x = _dataBase.Recipes.ToList()[index];
                if (nameList.Contains(x.Description) || string.IsNullOrWhiteSpace(x.Description))
                {
                    _dataBase.Recipes.Remove(x);
                }
                else
                {
                    nameList.Add(x.Description);
                }
            }

            _dataBase.SaveChanges();
        }


        public void LoadRecipes()
        {
            if (ACollection is null)
            {
                ACollection = new ObservableCollection<Recipe>(_dataBase.Recipes.ToList());
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
            if (string.IsNullOrWhiteSpace(NewRecipeName)) { return; }
            _dataBase.Recipes.Add(new Recipe() { Description = NewRecipeName });
            _dataBase.SaveChanges();
            _dataBase = new DataBase(); //otherwise recipe is always null if it's the first Recipe to be added.

            LoadRecipes();
            var newRecipe = ACollection.FirstOrDefault(x => x.Description == NewRecipeName);

            if (newRecipe is null)
            {
                return;
            }

            //SelectedRecipe = null;
            //OnPropertyChanged(nameof(SelectedRecipe));

            //SelectedRecipe = new RecipeDetailViewModel(newRecipe);

            LoadRecipes();
            SelectedRecipe = this.ACollection.Last();
            OnPropertyChanged(nameof(SelectedRecipe));



            NewRecipeName = "";
            OnPropertyChanged(nameof(NewRecipeName));
        }

    }
}
