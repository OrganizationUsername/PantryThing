using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Inventory
{
    public sealed class InventoryViewModel : VmBase
    {
        public ObservableCollection<LocationFoods> LocationFoodsCollection { get; set; }
        //public ObservableCollection<LocationFoodViewModel> LocationFoodsCollection { get; set; }

        public InventoryViewModel()
        {
            var db = new DataBase();

            var locationFoods = db.LocationFoods.Include(x => x.Item).ToList();
            LocationFoodsCollection = new(locationFoods);

            //foreach (var x in locationFoods)
            //{
            //    LocationFoodsCollection.Add(x);
            //    //LocationFoodsCollection.Add(new LocationFoodViewModel() { LocationFoods = x });
            //}
            OnPropertyChanged(nameof(LocationFoodsCollection));
        }

    }
}
