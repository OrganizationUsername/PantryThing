using System;
using System.Collections.Generic;
using System.Linq;

namespace Pantry.Core.Models
{
    public class PantryProvider 
    {
        private List<RecipeFood> _foodInstances;
        public PantryProvider(List<RecipeFood> foodInstances)
        {
            _foodInstances = foodInstances;
        }
        public List<RecipeFood> GetFoodInstances()
        {
            return _foodInstances;
        }

        public List<RecipeFood> AdjustOnHandQuantity(CookPlan canCook)
        {
            if (!canCook.CanMake)
            {
                return null;
            }

            if (canCook.RawCost is null)
            {
                return AdjustQuantity(canCook);
            }
            foreach (var rawCost in canCook.RawCost)
            {
                while (rawCost.Amount > 0)
                {
                    var pantryItem =
                        _foodInstances.First(pantry => pantry.Food == rawCost.Food && pantry.Amount > 0);
                    var amountToDeduct = Math.Min(pantryItem.Amount, rawCost.Amount);
                    pantryItem.Amount -= amountToDeduct;
                    rawCost.Amount -= amountToDeduct;
                }
            }
            _foodInstances = _foodInstances.Where(pi => pi.Amount > 0).ToList();
            return _foodInstances;
        }

        private List<RecipeFood> AdjustQuantity(CookPlan canCook)
        {
            foreach (var rawCost in canCook.TotalInput)
            {
                while (rawCost.Amount > 0)
                {
                    var pantryItem =
                        _foodInstances.First(pantry => pantry.Food == rawCost.Food && pantry.Amount > 0);
                    var amountToDeduct = Math.Min(pantryItem.Amount, rawCost.Amount);
                    pantryItem.Amount -= amountToDeduct;
                    rawCost.Amount -= amountToDeduct;
                }
            }

            _foodInstances.AddRange(canCook.TotalOutput);
            _foodInstances = _foodInstances.Where(pi => pi.Amount > 0).ToList();
            return _foodInstances;
        }
    }
}