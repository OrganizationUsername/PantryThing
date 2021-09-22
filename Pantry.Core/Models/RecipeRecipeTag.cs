namespace Pantry.Core.Models
{
    public class RecipeRecipeTag
    {
        public int RecipeRecipeTagId { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int RecipeTagId { get; set; }
        public RecipeTag RecipeTag { get; set; }
    }
}