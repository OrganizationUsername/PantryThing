using Pantry.Data;
using Pantry.ServiceGateways;
using Pantry.WPF.Equipment;
using Pantry.WPF.Food;
using Pantry.WPF.Inventory;
using Pantry.WPF.Item;
using Pantry.WPF.Recipe;
using Pantry.WPF.Shared;

namespace Pantry.WPF.Main
{
    public class MainWindowViewModel : VmBase
    {
        public VmBase MainView { get; set; }
        public string VmName { get; set; }
        public Seeder Seeder { get; set; }
        public NavigationCommand RecipeNavigationCommand { get; set; }
        public NavigationCommand InventoryNavigationCommand { get; set; }
        public NavigationCommand FoodNavigationCommand { get; set; }
        public NavigationCommand ItemNavigationCommand { get; set; }
        public NavigationCommand EquipmentNavigationCommand { get; set; }

        public DelegateCommand SeedDatabaseCommand { get; set; }


        public MainWindowViewModel()
        {
            DataBase db = new DataBase();
            db.Database.EnsureCreated();
            Seeder = new();
            RecipeNavigationCommand = new(this, new RecipesListViewModel());
            InventoryNavigationCommand = new(this, new InventoryViewModel());
            EquipmentNavigationCommand = new(this, new EquipmentViewModel());
            FoodNavigationCommand = new(this, new FoodListViewModel());
            ItemNavigationCommand = new(this, new ItemViewModel());
            SeedDatabaseCommand = new(Seeder.SeedDatabase);
        }

    }
}