using System;
using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; }
        public List<(DateTime startTime, DateTime endTime, string StepName)> BookedTimes { get; set; }
    }
}