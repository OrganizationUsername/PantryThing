using System;
// ReSharper disable UnusedMember.Global

namespace Pantry.Core.Models
{
    public class EquipmentCommitment
    {
        public int EquipmentCommitmentId { get; set; }
        public int RecipeStepId { get; set; }
        public RecipeStep RecipeStep { get; set; }
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}