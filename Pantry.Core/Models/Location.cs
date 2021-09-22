using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
    }
}