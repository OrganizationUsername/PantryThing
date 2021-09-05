using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Food
    {
        public int FoodId { get; set; }
        public string Name { get; set; }
        public List<BetterRecipe> BetterRecipes { get; set; }
        public static bool operator ==(Food lhs, Food rhs) => rhs is not null && lhs is not null && lhs.FoodId == rhs.FoodId;
        public static bool operator !=(Food lhs, Food rhs) => !(lhs.FoodId == rhs.FoodId);
    }
}