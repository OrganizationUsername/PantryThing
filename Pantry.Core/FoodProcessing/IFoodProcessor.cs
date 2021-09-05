using System.Collections.Generic;
using Pantry.Core.Models;

namespace Pantry.Core.FoodProcessing
{
    public interface IFoodProcessor
    {
        CookPlan
            CanCookSomething(IList<FoodInstance> foodInventory, Recipe recipe, IList<Recipe> recipes = null);
    }
}