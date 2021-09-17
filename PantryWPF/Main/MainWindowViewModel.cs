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
            //This should all probably be in another project.
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
            HardCodedRecipeRepository hrr = new HardCodedRecipeRepository();
            var hardCodedRecipes = hrr.GetRecipes().Select(x => new Recipe() { MainOutput = dbContext.Foods.Single(y => y.Name == x.MainOutput.Name),  });
            foreach (var y in hrr.GetRecipes())
            {
                foreach (var fi in y.Inputs) { fi.Food = null; }
                foreach (var fi in y.Outputs) { fi.Food = null; }
                dbContext.Add(y);
            }
            dbContext.SaveChanges();

            var x = dbContext.Recipes.ToListAsync();
        }
    }
}