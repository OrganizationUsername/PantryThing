using Microsoft.EntityFrameworkCore;
using Pantry.Data;
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
            MainView = new RecipesListViewModel();
            RecipeNavigationCommand = new NavigationCommand(this, new RecipesListViewModel());
            InventoryNavigationCommand = new NavigationCommand(this, new InventoryViewModel());
            DataBase dbContext = new DataBase();
            dbContext.Database.EnsureCreated();

            //This should all probably be in another project.
            HardCodedRecipeRepository hrr = new HardCodedRecipeRepository();
            foreach (var y in hrr.GetRecipes())
            {
                dbContext.Add(y);
            }
            dbContext.SaveChanges();

            var x = dbContext.BetterRecipes.ToListAsync();
        }
    }
}