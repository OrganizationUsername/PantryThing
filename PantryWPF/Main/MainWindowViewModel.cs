using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;
using Pantry.Data.UtilityFunctions;
using PantryWPF.Food;
using PantryWPF.Inventory;
using PantryWPF.Recipes;
using Unity;

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
        public DelegateCommand SeedDatabaseCommand { get; set; }


        public MainWindowViewModel()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<DataBase, DataBase>();
            Seeder = new();
            RecipeNavigationCommand = new(this, new RecipesListViewModel());
            InventoryNavigationCommand = new(this, new InventoryViewModel());
            FoodNavigationCommand = new(this, new FoodListViewModel());
            SeedDatabaseCommand = new(Seeder.SeedDatabase);
        }

    }
}