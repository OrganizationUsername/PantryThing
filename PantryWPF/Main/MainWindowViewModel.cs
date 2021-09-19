using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Data;
using PantryWPF.Food;
using PantryWPF.Inventory;
using PantryWPF.Recipes;
using Unity;

namespace PantryWPF.Main
{
    public class MainWindowViewModel : VmBase
    {
        public VmBase MainView { get; set; }
        public NavigationCommand RecipeNavigationCommand { get; set; }
        public NavigationCommand InventoryNavigationCommand { get; set; }
        public NavigationCommand FoodNavigationCommand { get; set; }
        public MainWindowViewModel()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<DataBase, DataBase>();
            var dbContext = new DataBase();

            MainView = new RecipesListViewModel();
            RecipeNavigationCommand = new NavigationCommand(this, new RecipesListViewModel());
            InventoryNavigationCommand = new NavigationCommand(this, new InventoryViewModel());
            FoodNavigationCommand = new NavigationCommand(this, new FoodListViewModel());
            var existingFoods = dbContext.Foods.Select(x => x.FoodName).ToList();
            //This should all probably be in another project.
    

            


            var x = dbContext.Recipes.ToListAsync();
        }

        
    }
}