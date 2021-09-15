using System;

namespace Pantry.Core.Models
{
    public class FoodInstance
    {
        public int Id { get; set; }
        public Food FoodType { get; set; }
        public int FoodTypeId { get; set; }
        public double Amount { get; set; }
        public DateTime Created { get; set; }
    }
}