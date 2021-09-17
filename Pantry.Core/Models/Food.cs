using System.Collections.Generic;

namespace Pantry.Core.Models
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public class Food
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public ICollection<RecipeFood> RecipeFoods { get; set; }

        public static bool operator ==(Food lhs, Food rhs) => rhs is not null && lhs is not null && lhs.FoodId == rhs.FoodId;
        public static bool operator !=(Food lhs, Food rhs) => !(lhs.FoodId == rhs.FoodId);
    }
}