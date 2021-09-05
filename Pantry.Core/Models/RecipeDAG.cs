using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class RecipeDAG
    {
        public BetterRecipe MainRecipe { get; set; }
        public List<RecipeDAG> SubordinateBetterRecipes = new();
        public bool Resolved = false;
    }
}