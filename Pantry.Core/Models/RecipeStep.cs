using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class RecipeStep
    {
        public int RecipeStepId { get; set; }
        public int RecipeId { get; set; }
        public int Order { get; set; }
        public string Instruction { get; set; }
        public double TimeCost { get; set; }
        public List<Equipment> Equipments { get; set; }
    }
}