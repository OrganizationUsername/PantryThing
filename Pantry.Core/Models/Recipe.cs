using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Description { get; set; }
        public FoodInstance OutputFoodInstance { get; set; }
        public List<FoodInstance> InputFoodInstance { get; set; }
        public double TimeCost { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
        public RecipeHierarchy RecipeHierarchy { get; set; }
    }
}