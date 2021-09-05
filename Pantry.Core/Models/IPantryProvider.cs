using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public interface IPantryProvider
    {
        List<FoodInstance> GetFoodInstances();
        List<FoodInstance> AdjustOnHandQuantity(CookPlan canCook);
    }
}