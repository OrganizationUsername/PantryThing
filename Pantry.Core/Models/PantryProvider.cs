using System;
using System.Collections.Generic;
using System.Linq;

namespace Pantry.Core.Models
{
    public class PantryProvider : IPantryProvider
    {
        private List<FoodInstance> _foodInstances;
        public PantryProvider(List<FoodInstance> foodInstances)
        {
            _foodInstances = foodInstances;
        }
        public List<FoodInstance> GetFoodInstances()
        {
            return _foodInstances;
        }

        public List<FoodInstance> AdjustOnHandQuantity(CookPlan canCook)
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
                        _foodInstances.First(pantry => pantry.FoodType == rawCost.FoodType && pantry.Amount > 0);
                    var amountToDeduct = Math.Min(pantryItem.Amount, rawCost.Amount);
                    pantryItem.Amount -= amountToDeduct;
                    rawCost.Amount -= amountToDeduct;
                }
            }
            _foodInstances = _foodInstances.Where(pi => pi.Amount > 0).ToList();
            return _foodInstances;
        }

        private List<FoodInstance> AdjustQuantity(CookPlan canCook)
        {
            foreach (var rawCost in canCook.TotalInput)
            {
                while (rawCost.Amount > 0)
                {
                    var pantryItem =
                        _foodInstances.First(pantry => pantry.FoodType == rawCost.FoodType && pantry.Amount > 0);
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