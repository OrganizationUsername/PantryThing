using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class RecipeDag
    {
        public BetterRecipe MainRecipe { get; set; }
        public IList<RecipeDag> SubordinateBetterRecipes = new List<RecipeDag>();
        public bool Scheduled = false;
    }
}