using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Pantry.Core.Models;
using PantryWPF.Main;
using ServiceGateways;

namespace PantryWPF.Inventory
{
    public sealed class InventoryViewModel : VmBase
    {
        private Location _selectedLocation;
        public ObservableCollection<LocationFoods> LocationFoodsCollection { get; set; }
        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Pantry.Core.Models.Item> Items { get; set; }
        public Pantry.Core.Models.Item SelectedItem { get; set; }
        private LocationFoods _selectedLocationFood;
        public DelegateCommand SaveChangesDelegateCommand { get; set; }
        public DelegateCommand AddLocationFoodDelegateCommand { get; set; }
        private readonly ItemService _itemService;

        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                if (_selectedLocation is null)
                {
                    OnPropertyChanged(nameof(SelectedLocation));
                    return;
                }
                OnPropertyChanged(nameof(SelectedLocation));
                LocationFoodsCollection = new(_itemService.GetLocationFoodsAtLocation(SelectedLocation.LocationId));
                OnPropertyChanged(nameof(LocationFoodsCollection));
            }
        }

        public LocationFoods SelectedLocationFood
        {
            get => _selectedLocationFood;
            set
            {
                _selectedLocationFood = value;
                OnPropertyChanged(nameof(SelectedLocationFood));
            }
        }

        public InventoryViewModel()
        {
            _itemService = new(); //TODO: This should be injected.
            SaveChangesDelegateCommand = new(SaveChanges);
            AddLocationFoodDelegateCommand = new(AddNewLocationFood);
            LoadData();
        }

        public void LoadData()
        {
            Locations = new(_itemService.GetLocations());
            OnPropertyChanged(nameof(Locations));
            LocationFoodsCollection = new(_itemService.GetLocationFoodsAtLocation(Locations.FirstOrDefault()));
            if (LocationFoodsCollection.Count == 0)
            {
                SelectedLocation = null;
            }
            else
            {
                SelectedLocation = Locations.First(x => x.LocationId == LocationFoodsCollection.FirstOrDefault()?.LocationId);
            }
            Items = new(_itemService.GetItems());
            OnPropertyChanged(nameof(LocationFoodsCollection));
        }

        public void AddNewLocationFood()
        {
            if (SelectedItem is null || SelectedLocation is null) { return; }
            _itemService.AddLocationFood(SelectedItem, SelectedLocation);
            ReLoadData();
        }

        public void ReLoadData()
        {
            LocationFoodsCollection.Clear();
            var xs = _itemService.GetLocationFoodsAtLocation(SelectedLocation.LocationId);
            foreach (var x in xs)
            {
                LocationFoodsCollection.Add(x);
            }
            SelectedLocationFood = null;
            OnPropertyChanged(nameof(LocationFoodsCollection));
        }

        public void SaveChanges()
        {
            if (SelectedLocationFood is null)
            {
                MessageBox.Show("An item must be selected.");
                return;
            }

            if (SelectedLocationFood?.Location is null)
            {
                MessageBox.Show("Selected item must have a location.");
                return;
            }

            _itemService.SaveLocationFood(SelectedLocationFood);
            ReLoadData();
        }

    }
}
