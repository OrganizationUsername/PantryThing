// ReSharper disable UnusedMember.Global
using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class EquipmentType
    {
        public int EquipmentTypeId { get; set; }
        public string EquipmentTypeName { get; set; }
        public int RecipeStepEquipmentTypeId { get; set; }
        public ICollection<RecipeStepEquipmentType> RecipeStepEquipmentType { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
    }
}