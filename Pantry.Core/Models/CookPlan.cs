using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class CookPlan
    {
        public string RecipeName;
        public bool CanMake;
        public IList<FoodInstance> TotalOutput;
        public IList<FoodInstance> TotalInput;
        public IList<FoodInstance> RawCost;
        public List<Recipe> RecipesTouched;
        public Queue<List<RecipeStep>> RecipeSteps;
        public RecipeDag RecipeDag;
        public string Steps;
    }
}