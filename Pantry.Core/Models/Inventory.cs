using System;

namespace Pantry.Core.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime DateExpired { get; set; }
        public double Remaining { get; set; }
    }
}