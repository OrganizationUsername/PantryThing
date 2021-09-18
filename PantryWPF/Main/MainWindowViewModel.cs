using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;
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
        public MainWindowViewModel()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IDataBase, DataBase>();
            var dbContext = new DataBase();

            MainView = new RecipesListViewModel();
            RecipeNavigationCommand = new NavigationCommand(this, new RecipesListViewModel());
            InventoryNavigationCommand = new NavigationCommand(this, new InventoryViewModel());
            var existingFoods = dbContext.Foods.Select(x => x.FoodName).ToList();
            //This should all probably be in another project.
    

            


            var x = dbContext.Recipes.ToListAsync();
        }
    }
}