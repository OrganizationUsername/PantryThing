// ReSharper disable UnusedMember.Global
using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class RecipeTag
    {
        public int RecipeTagId { get; set; }
        public string RecipeTagName { get; set; }
        public ICollection<RecipeRecipeTag> RecipeRecipeTags { get; set; }

    }
}