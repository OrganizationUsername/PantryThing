using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public int EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public ICollection<EquipmentCommitment> EquipmentCommitments { get; set; }
        public ICollection<RecipeStepEquipmentType> RecipeStepEquipment { get; set; }
    }

    public class EquipmentType
    {
        public int EquipmentTypeId { get; set; }
        public string EquipmentTypeName { get; set; }
        public int RecipeStepEquipmentTypeId { get; set; }
        public RecipeStepEquipmentType RecipeStepEquipmentType { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
    }

}