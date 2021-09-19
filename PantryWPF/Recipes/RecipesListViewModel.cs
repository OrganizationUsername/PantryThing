using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Recipes
{
    public class RecipesListViewModel : VmBase
    {
        private readonly DataBase _dataBase;
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
            for (var index = 0; index < _dataBase.Recipes.ToList().Count; index++)
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
            LoadRecipes();
            NewRecipeName = "";
        }

    }
}
