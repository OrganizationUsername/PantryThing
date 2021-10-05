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
        private Location _selectedLocation;
        public ObservableCollection<LocationFoods> LocationFoodsCollection { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        private readonly DataBase _db;

        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged(nameof(SelectedLocation));
                LocationFoodsCollection = new(_db.LocationFoods
                    .Where(x => x.Location.LocationName == SelectedLocation.LocationName)
                    .Include(x => x.Item).ToList());
                OnPropertyChanged(nameof(LocationFoodsCollection));
            }
        }

        public InventoryViewModel()
        {
            _db = new DataBase();
            LocationFoodsCollection = new(_db.LocationFoods.Include(x => x.Item).ToList());
            Locations = new ObservableCollection<Location>(_db.Locations.ToList());
            SelectedLocation = _db.Locations.First();
            OnPropertyChanged(nameof(LocationFoodsCollection));
        }





    }
}
