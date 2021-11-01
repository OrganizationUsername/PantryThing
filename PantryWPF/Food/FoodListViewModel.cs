using System.Linq;
using System.Windows;
using Pantry.ServiceGateways.Food;
using Pantry.WPF.Shared;
using Serilog.Core;
using Stylet;

namespace Pantry.WPF.Food
{
    public class FoodListViewModel : Screen
    {
        private readonly FoodServiceGateway _foodServiceGateway;
        private readonly Logger _logger;

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

        public DelegateCommand AddFoodCommand { get; set; }
        public DelegateCommand DeleteFoodCommand { get; set; }
        public DelegateCommand EditFoodCommand { get; set; }

        public FoodListViewModel(FoodServiceGateway foodServiceGateway, Logger logger)
        {
            _foodServiceGateway = foodServiceGateway;
            _logger = logger;
            AddFoodCommand = new(AddFood);
            DeleteFoodCommand = new(DeleteSelectedFood);
            EditFoodCommand = new(SaveSelectedFood);
        }

        public void SaveSelectedFood()
        {
            if (SelectedFood is null) return;
            if (!_foodServiceGateway.SaveSelectedFood(SelectedFood.FoodId, SelectedFood.IsEdible, SelectedFood.FoodName))
            {
                MessageBox.Show("Food not selected or save didn't work.");
            }
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
            _logger.Debug("FoodListViewModel.LoadData() Start");
            Foods.Clear();
            var tempList = _foodServiceGateway.GetAllFoods();
            if (tempList is null) return;
            Foods.AddRange(tempList);
            SelectedFood = Foods.FirstOrDefault();
            _logger.Debug("FoodListViewModel.LoadData() End");
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
