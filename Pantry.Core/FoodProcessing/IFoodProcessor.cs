using System.Collections.Generic;

namespace Pantry.Core.FoodProcessing
{
    public interface IFoodProcessor
    {
        CookPlan
            CanCookSomething(IList<FoodInstance> foodInventory, Recipe recipe, IList<Recipe> recipes = null);
    }
}