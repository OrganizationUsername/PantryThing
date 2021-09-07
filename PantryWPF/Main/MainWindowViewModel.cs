using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
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
            var dbContext = new DataBase();
            var existingFoods = dbContext.Foods.Select(x => x.Name).ToList();
            var hardCodedFoodRepository = new HardCodedFoodRepository();
            var hardCodedFoods = hardCodedFoodRepository.GetFoods().Select(x => new Food() { Name = x.Name });
            foreach (var y in hardCodedFoods)
            {
                if (!existingFoods.Contains(y.Name))
                {
                    dbContext.Add(y);
                }
            }
            dbContext.SaveChanges();

            //This should all probably be in another project.
            HardCodedRecipeRepository hrr = new HardCodedRecipeRepository();
            var hardCodedRecipes = hrr.GetRecipes().Select(x=> new BetterRecipe(){});
            foreach (var y in hrr.GetRecipes())
            {
                //dbContext.Add(y);
            }
            //dbContext.SaveChanges();

            var x = dbContext.BetterRecipes.ToListAsync();
        }
    }
}