using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFConsole
{
    //dotnet ef migrations add initial
    //dotnet ef database update 
    class Program
    {
        static void Main()
        {
            SeedFood();
            PrintRecipes();
            Console.ReadLine();
        }

        public static async void PrintRecipes()
        {
            var context = new SchoolContext();
            var recipes = await context.Recipes.Include(x => x.Steps).ThenInclude(x => x.EquipmentSteps).Include(x => x.RecipeFoods).ThenInclude(x => x.Food).ToListAsync();

            List<string> recipeLines = new();
            foreach (var x in recipes)
            {
                List<string> temporaryRecipeLines = new();
                temporaryRecipeLines.Add($"{x.Name}- {x.RecipeId}");
                temporaryRecipeLines.Add("Ingredients:");
                foreach (var y in x.RecipeFoods)
                {
                    temporaryRecipeLines.Add($"\t{y.Amount}: {y.Food.Name}");
                }
                temporaryRecipeLines.Add("Method:");
                foreach (var y in x.Steps.OrderBy(x => x.StepId))
                {
                    temporaryRecipeLines.Add($"{y.TimeCost}: {y.Instruction}");
                    if (y.EquipmentSteps.Any())
                    {
                        temporaryRecipeLines.Add($"\t{string.Join(",", y.EquipmentSteps.Select(x => x.Name))}");
                    }
                }
                recipeLines.Add(string.Join(Environment.NewLine, temporaryRecipeLines));
            }
            Console.WriteLine(string.Join($"{Environment.NewLine}-----{Environment.NewLine}", recipeLines));
        }

        public static async void SeedFood()
        {
            var context = new SchoolContext();

            if (!context.Foods.AnyAsync().Result)
            {
                var foods = new Food[]
                {
                    new() {Name = "Sandwich"},
                    new() {Name = "Cake"},
                    new() {Name = "Bread"},
                    new() {Name = "Flour"},
                    new() {Name = "Meat"},
                    new() {Name = "Eggs"},
                    new() {Name = "Milk"},
                };
                foreach (var s in foods)
                {
                    await context.Foods.AddAsync(s);
                }
            }

            if (!context.Recipes.AnyAsync().Result)
            {
                var recipes = new Recipe[]
                {
                new(){Name = "Sandwich", RecipeId = 1},
                new(){Name = "Cake", RecipeId = 2},
                };
                foreach (var s in recipes)
                {
                    await context.Recipes.AddAsync(s);
                }
            }

            if (!context.RecipeFoods.AnyAsync().Result)
            {
                var recipeFoods = new RP[]
                {
                    new() {Amount = 002, RecipeId = 1, FoodId = 3},
                    new() {Amount = 001, RecipeId = 1, FoodId = 5},
                    new() {Amount = 100, RecipeId = 2, FoodId = 4},
                    new() {Amount = 003, RecipeId = 2, FoodId = 6},
                    new() {Amount = 050, RecipeId = 2, FoodId = 7},
                };
                foreach (var s in recipeFoods)
                {
                    await context.RecipeFoods.AddAsync(s);
                }
            }

            if (!context.Steps.AnyAsync().Result)
            {
                var steps = new Step[]
                {
                    new() {RecipeId = 1, TimeCost = 1, Instruction = "Get Bread", StepOrder = 10},
                    new() {RecipeId = 1, TimeCost = 2, Instruction = "Combine", StepOrder = 20},
                    new() {RecipeId = 2, TimeCost = 10, Instruction = "Get Ingredients together", StepOrder = 10},
                    new() {RecipeId = 2, TimeCost = 60, Instruction = "Bake", StepOrder = 20},
                };
                foreach (var s in steps)
                {
                    await context.Steps.AddAsync(s);
                }
            }

            if (!context.EquipmentSteps.AnyAsync().Result)
            {
                var equipmentSteps = new EquipmentStep[]
                {
                    new() {Name = "Bread Machine", StepId = 4},
                };
                foreach (var s in equipmentSteps)
                {
                    await context.EquipmentSteps.AddAsync(s);
                }
            }

            await context.SaveChangesAsync();
        }
        
    }

    public class Food
    {
        public int FoodId { get; set; }
        public string Name { get; set; }

        public ICollection<RP> RecipeFoods { get; set; }
    }

    public class RP
    {
        public int RecipeFoodId { get; set; }
        public int FoodId { get; set; }
        public int RecipeId { get; set; }
        public double Amount { get; set; }
        public Food Food { get; set; }
        public Recipe Recipe { get; set; }
    }

    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public ICollection<RP> RecipeFoods { get; set; }
        public ICollection<Step> Steps { get; set; }
    }

    public class Step
    {
        public int StepId { get; set; }
        public double TimeCost { get; set; }
        public string Instruction { get; set; }
        public int RecipeId { get; set; }
        public int StepOrder { get; set; }
        public Recipe Recipe { get; set; }
        public ICollection<EquipmentStep> EquipmentSteps { get; set; }
    }

    public class EquipmentStep
    {
        public int EquipmentStepId { get; set; }
        public int StepId { get; set; }
        public string Name { get; set; }
    }

    public class SchoolContext : DbContext
    {
        public DbSet<Food> Foods { get; set; }
        public DbSet<RP> RecipeFoods { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<EquipmentStep> EquipmentSteps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\Programming\EFTesting\EFConsole\testDb.db;");
        }
    }
}
