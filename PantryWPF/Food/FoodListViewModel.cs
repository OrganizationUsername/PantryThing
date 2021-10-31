using System;
using System.Linq;
using Pantry.Data;
using Pantry.ServiceGateways.Recipe;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Food
{
    public class FoodListViewModel : Screen
    {
        private readonly FoodServiceGateWay _foodServiceGateway;

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

        public FoodListViewModel(FoodServiceGateWay foodServiceGateway)
        {
            _foodServiceGateway = foodServiceGateway;
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
            if (_selectedFood is null) return;
            _foodServiceGateway.DeleteFood(_selectedFood.FoodId);
            LoadData();
        }

        public void GetSelectedRecipes()
        {
            if (SelectedFood is null)
            {
                Recipes = new();
                OnPropertyChanged(nameof(Recipes));
                return;
            }

            var recipeList = _foodServiceGateway.GetRecipes(SelectedFood);
            Recipes.Clear();
            Recipes.AddRange(recipeList);
            OnPropertyChanged(nameof(Recipes));
        }

        public void KeepOnlyUniqueFoodNames()
        {
            _foodServiceGateway.KeepOnlyUniqueFoodNames();
        }

        public void LoadData()
        {
            Foods.Clear();
            var tempList = _foodServiceGateway.GetAllFoods();
            if (tempList is null) return;
            Foods.AddRange(tempList);
            SelectedFood = Foods.FirstOrDefault();
        }

        public void AddFood()
        {
            if (string.IsNullOrWhiteSpace(NewFoodName)) return;
            _foodServiceGateway.AddFood(NewFoodName);
            NewFoodName = "";
            LoadData();
            SelectedFood = Foods.Last();
        }
    }
}
