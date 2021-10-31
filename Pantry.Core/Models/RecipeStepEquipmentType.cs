// ReSharper disable UnusedMember.Global
namespace Pantry.Core.Models
{
    public class RecipeStepEquipmentType
    {
        public int RecipeStepEquipmentTypeId { get; set; }
        public RecipeStep RecipeStep { get; set; }
        public int RecipeStepId { get; set; }
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
        public int EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; }

    }
}