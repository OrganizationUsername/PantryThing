using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class BetterRecipe
    {
        public int RecipeId { get; set; }
        public List<FoodInstance> Inputs { get; set; }
        public List<FoodInstance> Outputs { get; set; }
        public Food MainOutput { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
    }
}