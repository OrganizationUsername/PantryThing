using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Food
{
    public class FoodListViewModel : VmBase
    {
        private readonly DataBase _dataBase;
        private Pantry.Core.Models.Food _selectedFood;

        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; } = new();
        public ObservableCollection<Pantry.Core.Models.Recipe> Recipes { get; set; } = new();
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
            _dataBase = new();
            KeepOnlyUniqueFoodNames();
            LoadData();
            AddRecipeCommand = new(AddFood);
            DeleteFoodCommand = new(DeleteSelectedFood);
        }

        public void DeleteSelectedFood()
        {
            if (_selectedFood is not null)
            {
                var foodToDelete = _dataBase.Foods.First(x => x.FoodId == _selectedFood.FoodId);
                _dataBase.Foods.Remove(foodToDelete);
                _dataBase.SaveChanges();
                LoadData();
            }
        }

        public void GetSelectedRecipes()
        {
            if (_selectedFood is null || _selectedFood.RecipeFoods is null)
            {
                Recipes = new();
                return;
            }
            var tempList = _selectedFood.RecipeFoods.Where(x => x.Amount > 0).Select(x => x.Recipe).Distinct().ToList();
            Recipes.Clear();
            foreach (var x in tempList)
            {
                Recipes.Add(x);
            }
            OnPropertyChanged(nameof(Recipes));
        }

        public void KeepOnlyUniqueFoodNames()
        {
            var names = new List<string>();
            foreach (var x in _dataBase.Foods)
            {
                if (!names.Contains(x.FoodName) && !string.IsNullOrWhiteSpace(x.FoodName))
                {
                    names.Add(x.FoodName);
                }
                else
                {
                    _dataBase.Foods.Remove(x);
                }
            }
            _dataBase.SaveChanges();
        }


        public void LoadData()
        {

            if (_dataBase.Foods is null)
            {
                Foods = new();
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
            _dataBase.Foods.Add(new() { FoodName = NewFoodName });
            _dataBase.SaveChanges();
            NewFoodName = "";
            OnPropertyChanged(nameof(NewFoodName));
            LoadData();
            SelectedFood = Foods.Last();
        }

    }
}
