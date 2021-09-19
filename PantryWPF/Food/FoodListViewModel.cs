using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Food
{
    public class FoodListViewModel : VmBase
    {
        private readonly DataBase _dataBase;
        private Pantry.Core.Models.Food _selectedFood;

        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; } = new();
        public ObservableCollection<Recipe> Recipes { get; set; } = new();
        public string NewFoodName { get; set; }
        public Pantry.Core.Models.Food SelectedFood
        {
            get => _selectedFood;
            set
            {
                _selectedFood = value;
                OnPropertyChanged(nameof(SelectedFood));
                GetSelectedRecipes();
            }
        }

        public DelegateCommand AddRecipeCommand { get; set; }
        public DelegateCommand DeleteFoodCommand { get; set; }
        public FoodListViewModel()
        {
            _dataBase = new DataBase();
            KeepOnlyUniqueFoodNames();
            LoadFoods();
            AddRecipeCommand = new DelegateCommand(AddFood);
            DeleteFoodCommand = new DelegateCommand(DeleteSelectedFood);
        }

        public void DeleteSelectedFood()
        {
            if (_selectedFood is not null)
            {
                var foodToDelete = _dataBase.Foods.First(x => x.FoodId == _selectedFood.FoodId);
                _dataBase.Foods.Remove(foodToDelete);
                _dataBase.SaveChanges();
                LoadFoods();
            }
        }

        public void GetSelectedRecipes()
        {
            if (_selectedFood is null || _selectedFood.RecipeFoods is null)
            {
                Recipes = new();
                return;
            }
            var tempList = _selectedFood.RecipeFoods.Select(x => x.Recipe).Distinct().ToList();
            Recipes.Clear();
            foreach (var x in tempList)
            {
                Recipes.Add(x);
            }
            OnPropertyChanged(nameof(Recipes));
        }

        public void KeepOnlyUniqueFoodNames()
        {
            var Names = new List<string>();
            foreach (var x in _dataBase.Foods)
            {
                if (!Names.Contains(x.FoodName) && !string.IsNullOrWhiteSpace(x.FoodName))
                {
                    Names.Add(x.FoodName);
                }
                else
                {
                    _dataBase.Foods.Remove(x);
                }
            }
            _dataBase.SaveChanges();
        }


        public void LoadFoods()
        {

            if (_dataBase.Foods is null)
            {
                Foods = new ObservableCollection<Pantry.Core.Models.Food>();
                OnPropertyChanged(nameof(Foods));
                return;
            }

            Foods.Clear();

            foreach (var x in _dataBase.Foods.Include(x => x.RecipeFoods).ThenInclude(x => x.Recipe))
            {
                Foods.Add(x);
            }
            SelectedFood = Foods.FirstOrDefault();
        }

        public void AddFood()
        {
            if (string.IsNullOrWhiteSpace(NewFoodName)) { return; }
            _dataBase.Foods.Add(new Pantry.Core.Models.Food() { FoodName = NewFoodName });
            _dataBase.SaveChanges();
            NewFoodName = "";
            OnPropertyChanged(nameof(NewFoodName));
            LoadFoods();
            SelectedFood = Foods.Last();
        }

    }
}
