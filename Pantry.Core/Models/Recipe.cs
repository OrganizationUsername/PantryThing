using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public List<RecipeFood> Inputs { get; set; }
        public List<RecipeFood> Outputs { get; set; }
        public Food MainOutput { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
        public string Description { get; set; }
    }
}