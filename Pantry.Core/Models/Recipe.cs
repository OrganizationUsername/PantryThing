using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public List<RecipeFood> RecipeFoods { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
        public ICollection<RecipeRecipeTag> RecipeRecipeTags { get; set; }
        public string Description { get; set; }
    }
}