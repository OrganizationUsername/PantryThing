using System.Linq;
using System.Threading.Tasks;
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
                GetSelectedRecipes().GetAwaiter().GetResult();
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

        public async Task SaveSelectedFood()
        {
            if (SelectedFood is null) return;

            bool saved = await _foodServiceGateway.SaveSelectedFood(SelectedFood.FoodId, SelectedFood.IsEdible, SelectedFood.FoodName);

            if (!saved)
            {
                MessageBox.Show("Food not selected or save didn't work.");
            }
        }

        protected async override void OnActivate()
        {
            base.OnActivate();
            await KeepOnlyUniqueFoodNames ();
            await LoadData();
        }

        public async Task DeleteSelectedFood()
        {
            if (_selectedFood is null) return;
            await _foodServiceGateway.DeleteFood(_selectedFood.FoodId);
            await LoadData();
        }

        public async Task GetSelectedRecipes()
        {
            if (SelectedFood is null)
            {
                Recipes = new();
                OnPropertyChanged(nameof(Recipes));
                return;
            }

            var recipeList = await _foodServiceGateway.GetRecipes(SelectedFood);
            Recipes.Clear();
            Recipes.AddRange(recipeList);
            OnPropertyChanged(nameof(Recipes));
        }

        public async Task KeepOnlyUniqueFoodNames()
        {
            await _foodServiceGateway.KeepOnlyUniqueFoodNames();
        }

        public async Task LoadData()
        {
            _logger.Debug("FoodListViewModel.LoadData() Start");
            Foods.Clear();
            var tempList = await _foodServiceGateway.GetAllFoods();
            if (tempList is null) return;
            Foods.AddRange(tempList);
            SelectedFood = Foods.FirstOrDefault();
            _logger.Debug("FoodListViewModel.LoadData() End");
        }

        public async Task AddFood()
        {
            if (string.IsNullOrWhiteSpace(NewFoodName)) return;
            await _foodServiceGateway.AddFood(NewFoodName);
            NewFoodName = "";
            await LoadData();
            SelectedFood = Foods.Last();
        }
    }
}
