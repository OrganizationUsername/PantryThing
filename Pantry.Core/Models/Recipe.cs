using System.Collections.Generic;
// ReSharper disable UnusedMember.Global
namespace Pantry.Core.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public List<RecipeFood> RecipeFoods { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
        public ICollection<RecipeRecipeTag> RecipeRecipeTags { get; set; }
        public ICollection<PlannedCook> PlannedCooks { get; set; }
        public string Description { get; set; }
    }
}