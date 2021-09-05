using System.Collections.Generic;

namespace Pantry.Core.Models
{
    public class RecipeHierarchy
    {
        //maybe each task should have a bunch of children. When traversing,
        //the top parent is usually "plate and serve the food"
        //then the children are all of the last steps required. iterate and
        //schedule all of them, once they are all scheduled, schedule their children
        public List<RecipeHierarchy> Dependents { get; set; }
        public string Instruction { get; set; }
        public double TimeCost { get; set; }
        public List<Equipment> Equipments { get; set; }
    }
}