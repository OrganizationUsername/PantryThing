using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public interface IPantryProvider
    {
        List<RecipeFood> GetFoodInstances();
        List<RecipeFood> AdjustOnHandQuantity(CookPlan canCook);
    }
}