// ReSharper disable UnusedMember.Global
using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class CookPlan
    {
        public string RecipeName;
        public bool CanMake;
        public IList<RecipeFood> TotalOutput;
        public IList<RecipeFood> TotalInput;
        public IList<RecipeFood> RawCost;
        public List<Recipe> RecipesTouched;
        public Queue<List<RecipeStep>> RecipeSteps;
        public RecipeDag RecipeDag;
        public string Steps;
    }
}