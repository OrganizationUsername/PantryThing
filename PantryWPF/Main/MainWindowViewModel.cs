using Pantry.Data.UtilityFunctions;
using PantryWPF.Food;
using PantryWPF.Inventory;
using PantryWPF.Item;
using PantryWPF.Recipe;

namespace PantryWPF.Main
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
        public DelegateCommand SeedDatabaseCommand { get; set; }


        public MainWindowViewModel()
        {
            Seeder = new();
            RecipeNavigationCommand = new(this, new RecipesListViewModel());
            InventoryNavigationCommand = new(this, new InventoryViewModel());
            FoodNavigationCommand = new(this, new FoodListViewModel());
            ItemNavigationCommand = new(this, new ItemViewModel());
            SeedDatabaseCommand = new(Seeder.SeedDatabase);
        }

    }
}