using System;

namespace Pantry.Core.Models
{
    public class FoodInstance
    {
        public Food FoodType { get; set; }
        public double Amount { get; set; }
        public DateTime Created { get; set; }
    }
}