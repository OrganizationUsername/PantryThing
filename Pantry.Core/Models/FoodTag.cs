using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class FoodTag
    {
        public int FoodTagId { get; set; }
        public ICollection<FoodFoodTag> FoodFoodTags { get; set; }
        public string FoodTagName { get; set; }

    }
}