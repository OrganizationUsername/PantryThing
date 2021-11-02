using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Pantry.Core.Models;
using Pantry.ServiceGateways;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Inventory
{
    public sealed class InventoryViewModel : Screen
    {
        private readonly ItemService _itemService;

        public BindableCollection<LocationFoods> LocationFoodsCollection { get; set; } = new();
        public BindableCollection<Core.Models.Location> Locations { get; set; } = new();
        public BindableCollection<Pantry.Core.Models.Item> Items { get; set; }

        public DelegateCommand SaveChangesDelegateCommand { get; set; }
        public DelegateCommand AddLocationFoodDelegateCommand { get; set; }

        private Pantry.Core.Models.Item _selectedItem;
        public Pantry.Core.Models.Item SelectedItem
        {
            get => _selectedItem;
            set => SetAndNotify(ref _selectedItem, value, nameof(SelectedItem));
        }

        private Core.Models.Location _selectedLocation;
        public Core.Models.Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (SetAndNotify(ref _selectedLocation, value, nameof(SelectedLocation)) && _selectedLocation is not null)
                {
                    LocationFoodsCollection.Clear();
                    LocationFoodsCollection.AddRange(_itemService.GetLocationFoodsAtLocation(SelectedLocation.LocationId).GetAwaiter().GetResult());
                    NotifyOfPropertyChange(nameof(LocationFoodsCollection));
                }
            }
        }

        private LocationFoods _selectedLocationFood;
        public LocationFoods SelectedLocationFood
        {
            get => _selectedLocationFood;
            set => SetAndNotify(ref _selectedLocationFood, value, nameof(SelectedLocationFood));
        }

        public InventoryViewModel(ItemService itemService)
        {
            _itemService = itemService;
            SaveChangesDelegateCommand = new(SaveChanges);
            AddLocationFoodDelegateCommand = new(AddNewLocationFood);
            LoadData().GetAwaiter().GetResult();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadData();
        }

        public async Task LoadData()
        {
            Locations.Clear();
            Locations.AddRange(await _itemService.GetLocations());
            if (Locations.Count == 0) return;
            LocationFoodsCollection = new(await _itemService.GetLocationFoodsAtLocation(Locations.First().LocationId));
            SelectedLocation = LocationFoodsCollection.Count == 0 ? null : Locations.First(x => x.LocationId == LocationFoodsCollection.FirstOrDefault()?.LocationId);
            Items = new(await _itemService.GetItems());
        }

        public async Task AddNewLocationFood()
        {
            if (SelectedItem is null || SelectedLocation is null) { return; }
            await _itemService.AddLocationFood(SelectedItem, SelectedLocation);
            await ReLoadData();
        }

        public async Task ReLoadData()
        {
            LocationFoodsCollection.Clear();
            LocationFoodsCollection.AddRange(await _itemService.GetLocationFoodsAtLocation(SelectedLocation.LocationId));
            SelectedLocationFood = null;
        }

        public async Task SaveChanges()
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

            await _itemService.SaveLocationFood(SelectedLocationFood);
            ReLoadData();
        }

    }
}
