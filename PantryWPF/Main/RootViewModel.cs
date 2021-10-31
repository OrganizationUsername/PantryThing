using System;
using Pantry.ServiceGateways;
using Pantry.WPF.Equipment;
using Pantry.WPF.Food;
using Pantry.WPF.Inventory;
using Pantry.WPF.Item;
using Pantry.WPF.Recipe;
using Pantry.WPF.Shared;
using Serilog.Core;
using Stylet;

namespace Pantry.WPF.Main
{
    public class RootViewModel : Conductor<IScreen>.StackNavigation
    {
        private readonly Logger _logger;
        private string _vmName;
        public string VmName
        {
            get => _vmName;
            set => SetAndNotify(ref _vmName, value, nameof(VmName));
        }

        public NavigationCommand RecipeNavigationCommand { get; set; }
        public NavigationCommand InventoryNavigationCommand { get; set; }
        public NavigationCommand FoodNavigationCommand { get; set; }
        public NavigationCommand ItemNavigationCommand { get; set; }
        public NavigationCommand EquipmentNavigationCommand { get; set; }
        public NavigationCommand EquipmentTypeNavigationCommand { get; set; }

        public DelegateCommand SeedDatabaseCommand { get; set; }


        public RootViewModel(
            RecipesListViewModel recipesListViewModel,
            InventoryViewModel inventoryViewModel,
            EquipmentViewModel equipmentViewModel,
            FoodListViewModel foodListViewModel,
            ItemViewModel itemViewModel,
            EquipmentTypeViewModel equipmentTypeViewModel,
            Seeder seed,
            Logger logger)
        {
            _logger = logger;
            RecipeNavigationCommand = new(this, recipesListViewModel);
            InventoryNavigationCommand = new(this, inventoryViewModel);
            EquipmentNavigationCommand = new(this, equipmentViewModel);
            FoodNavigationCommand = new(this, foodListViewModel);
            ItemNavigationCommand = new(this, itemViewModel);
            EquipmentTypeNavigationCommand = new(this, equipmentTypeViewModel);
            SeedDatabaseCommand = new(seed.SeedDatabase);

            _logger.Debug($"Started at {DateTime.Now}");
        }
    }
}