// ReSharper disable UnusedMember.Global
namespace Pantry.Core.Models
{
    public class FoodFoodTag
    {
        public int FoodFoodTagId { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }
        public int FoodTagId { get; set; }
        public FoodTag FoodTag { get; set; }
        public string TagName { get; set; }
    }
}