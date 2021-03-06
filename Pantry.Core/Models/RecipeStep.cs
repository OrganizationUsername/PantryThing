using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace Pantry.Core.Models
{
    public class RecipeStep
    {
        public int RecipeStepId { get; set; }
        public Recipe Recipe { get; set; }
        public int RecipeId { get; set; }
        public int Order { get; set; }
        public string Instruction { get; set; }
        public double TimeCost { get; set; }
        //public ICollection<Equipment> Equipments { get; set; } //I need to get rid of this one.
        public ICollection<RecipeStepEquipmentType> RecipeStepEquipmentType { get; set; }
    }
}