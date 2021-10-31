using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public int EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public ICollection<EquipmentCommitment> EquipmentCommitments { get; set; }
        public ICollection<RecipeStepEquipmentType> RecipeStepEquipment { get; set; }
    }
}