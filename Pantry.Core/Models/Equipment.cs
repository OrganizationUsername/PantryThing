using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public Location Location { get; set; }
        public int LocationId { get; set; }
        //public List<(DateTime startTime, DateTime endTime, string StepName)> BookedTimes { get; set; }
        public ICollection<EquipmentCommitment> EquipmentCommitments { get; set; }
    }
}