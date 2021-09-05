using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class RecipeDAG
    {
        public BetterRecipe MainRecipe { get; set; }
        public IList<RecipeDAG> SubordinateBetterRecipes = new List<RecipeDAG>();
        public bool Scheduled = false;
    }
}