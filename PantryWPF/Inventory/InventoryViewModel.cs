using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Inventory
{
    public class InventoryViewModel : VmBase
    {
        public ObservableCollection<LocationFoods> LocationFoodsCollection { get; set; }

        public InventoryViewModel()
        {
            LocationFoodsCollection = new();
            var db = new DataBase();

            var locationFoods = db.LocationFoods.Include(x => x.Item).ToList();
            foreach (var x in locationFoods)
            {
                LocationFoodsCollection.Add(x);
            }
            OnPropertyChanged(nameof(LocationFoodsCollection));
        }

    }
}
