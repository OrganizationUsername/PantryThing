using System;

namespace Pantry.Core.Models
{
    public class LocationFoods
    {
        public int LocationFoodsId { get; set; }
        public Food Food { get; set; }
        public int FoodId { get; set; }
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public double Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Exists { get; set; }

        public bool IsSpoiledAtTime(DateTime check)
        {
            if (ExpiryDate == DateTime.MinValue)
            {
                return false;
            }

            return (ExpiryDate + TimeSpan.FromDays(1) <= check);
        }

        public bool IsOkayAtTime(DateTime check)
        {
            if (ExpiryDate == DateTime.MinValue)
            {
                return false;
            }

            var a = !(ExpiryDate + TimeSpan.FromDays(1) < check);
            var b = PurchaseDate < check; //Doesn't yet exist

            return a && b;
        }
    }
}