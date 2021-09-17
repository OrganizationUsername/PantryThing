using System;
using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class RecipeFood
    {
        public int RecipeFoodId { get; set; }
        public double Amount { get; set; }
        public Food Food { get; set; }
        public int FoodId { get; set; }
        public Recipe Recipe { get; set; }
        public int RecipeId { get; set; }
    }
}