using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Navigation;
using PantryWPF.Inventory;
using PantryWPF.Recipes;

namespace PantryWPF.Main
{
    public class MainWindowViewModel : VmBase
    {
        public VmBase MainView { get; set; }
        public NavigationCommand RecipeNavigationCommand { get; set; }
        public NavigationCommand InventoryNavigationCommand { get; set; }
        public MainWindowViewModel()
        {
            MainView = new RecipesViewModel();
            RecipeNavigationCommand = new NavigationCommand(this, new RecipesViewModel());
            InventoryNavigationCommand = new NavigationCommand(this, new InventoryViewModel());
        }
    }
}