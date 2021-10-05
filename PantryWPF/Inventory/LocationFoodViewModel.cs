using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pantry.Core.Models;
using PantryWPF.Main;

namespace PantryWPF.Inventory
{
    public class LocationFoodViewModel : VmBase
    {
        public LocationFoods LocationFoods { get; set; }
        public int ANumber { get; set; } = new Random().Next(1, 100);

        public LocationFoodViewModel()
        {

        }
    }
}
