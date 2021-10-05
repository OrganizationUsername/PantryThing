using System;
using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public ICollection<Item> Items { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime DateExpired { get; set; }
        public double Remaining { get; set; }
    }
}