using System.Collections.Generic;
using Pantry.Core.Models;

namespace Pantry.Core.FoodProcessing
{
    public interface IFoodProcessor
    {
        CookPlan
            CanCookSomething(IList<RecipeFood> foodInventory, Recipe recipe, IList<Recipe> recipes = null);
    }
}