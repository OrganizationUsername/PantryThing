using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Data;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Food
{
    public class FoodListViewModel : Screen
    {
        private readonly Func<DataBase> _dbFactory;

        public BindableCollection<Pantry.Core.Models.Food> Foods { get; set; } = new();
        public BindableCollection<Pantry.Core.Models.Recipe> Recipes { get; set; } = new();

        private string _newFoodName;
        public string NewFoodName
        {
            get => _newFoodName;
            set => SetAndNotify(ref _newFoodName, value, nameof(NewFoodName));
        }

        private Pantry.Core.Models.Food _selectedFood;
        public Pantry.Core.Models.Food SelectedFood
        {
            get => _selectedFood;
            set
            {
                SetAndNotify(ref _selectedFood, value, nameof(SelectedFood));
                GetSelectedRecipes();
            }
        }

        public DelegateCommand AddRecipeCommand { get; set; }
        public DelegateCommand DeleteFoodCommand { get; set; }

        public FoodListViewModel(Func<DataBase> dbFactory)
        {
            _dbFactory = dbFactory;
            AddRecipeCommand = new(AddFood);
            DeleteFoodCommand = new(DeleteSelectedFood);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            KeepOnlyUniqueFoodNames();
            LoadData();
        }

        public void DeleteSelectedFood()
        {
            if (_selectedFood is not null)
            {
                using var ctx = _dbFactory();
                var foodToDelete = ctx.Foods.First(x => x.FoodId == _selectedFood.FoodId);
                ctx.Foods.Remove(foodToDelete);
                ctx.SaveChanges();
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
            Recipes.AddRange(tempList);
        }

        public void KeepOnlyUniqueFoodNames()
        {
            var names = new List<string>();
            using var ctx = _dbFactory();
            foreach (var x in ctx.Foods)
            {
                if (!names.Contains(x.FoodName) && !string.IsNullOrWhiteSpace(x.FoodName))
                {
                    names.Add(x.FoodName);
                }
                else
                {
                    ctx.Foods.Remove(x);
                }
            }
            ctx.SaveChanges();
        }


        public void LoadData()
        {
            using var ctx = _dbFactory();
            if (ctx.Foods is null)
            {
                Foods = new();
                return;
            }

            Foods.Clear();
            Foods.AddRange(ctx.Foods.Include(x => x.RecipeFoods).ThenInclude(x => x.Recipe));
            SelectedFood = Foods.FirstOrDefault();
        }

        public void AddFood()
        {
            if (string.IsNullOrWhiteSpace(NewFoodName)) { return; }
            using var ctx = _dbFactory();
            ctx.Foods.Add(new() { FoodName = NewFoodName });
            ctx.SaveChanges();
            NewFoodName = "";
            LoadData();
            SelectedFood = Foods.Last();
        }

    }
}
