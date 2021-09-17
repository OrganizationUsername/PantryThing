using System;
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

    public class Location
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
    }
    public class EquipmentCommitment
    {
        public int EquipmentCommitmentId { get; set; }
        public int RecipeStepId { get; set; }
        public RecipeStep RecipeStep { get; set; }
        public int RecipeId { get; set; }
        public Equipment Equipment { get; set; }
        public int EquipmentId { get; set; }
        public string Description { get; set; }
        public Recipe Recipe { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

}