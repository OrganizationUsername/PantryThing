using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
        public ObservableCollection<Item> Items { get; set; }
        public Item SelectedItem { get; set; }
        private readonly DataBase _db;
        private LocationFoods _selectedLocationFood;
        public DelegateCommand SaveChangesDelegateCommand { get; set; }
        public DelegateCommand AddLocationFoodDelegateCommand { get; set; }

        public LocationFoods SelectedLocationFood
        {
            get => _selectedLocationFood;
            set
            {
                if (_db.ChangeTracker.HasChanges())
                {
                    RejectChanges();
                    //A lot of this would be better if I used a projection.
                    return;
                }
                _selectedLocationFood = value;
                OnPropertyChanged(nameof(SelectedLocationFood));
            }
        }

        public void RejectChanges()
        {
            foreach (var entry in _db.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }

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
                LocationFoodsCollection = new(_db.LocationFoods
                    .Where(x => x.Location.LocationName == SelectedLocation.LocationName && x.Quantity > 0)
                    .Include(x => x.Item).ThenInclude(x => x.Food).ToList());
                OnPropertyChanged(nameof(LocationFoodsCollection));
            }
        }

        public InventoryViewModel()
        {
            SaveChangesDelegateCommand = new(SaveChanges);
            AddLocationFoodDelegateCommand = new(AddNewLocationFood);
            _db = new();
            LoadData();
            OnPropertyChanged(nameof(LocationFoodsCollection));
        }

        public void LoadData()
        {
            LocationFoodsCollection = new(_db.LocationFoods.Where(x => x.Quantity > 0).Include(x => x.Item).ToList());
            Locations = new(_db.Locations.ToList());
            SelectedLocation = LocationFoodsCollection.FirstOrDefault()?.Location;
            Items = new(_db.Items.ToList());
        }

        public void AddNewLocationFood()
        {
            if (SelectedItem is null || SelectedLocation is null) { return; }
            RejectChanges();
            _db.LocationFoods.Add(new()
            {
                PurchaseDate = DateTime.Now,
                Item = SelectedItem,
                Exists = true,
                Location = SelectedLocation,
                ExpiryDate = DateTime.MinValue,
                OpenDate = DateTime.MinValue,
                Quantity = _db.Items.Single(x => x.FoodId == SelectedItem.FoodId).Weight
            });
            _db.SaveChanges();
            ReLoadData();
        }

        public void ReLoadData()
        {
            LocationFoodsCollection.Clear();
            foreach (var x in (_db.LocationFoods.Include(x => x.Item).Where(x => x.Location.LocationName == SelectedLocation.LocationName).ToList()))
            {
                LocationFoodsCollection.Add(x);
            }
            SelectedLocationFood = null;
            OnPropertyChanged(nameof(Locations));
            OnPropertyChanged(nameof(LocationFoodsCollection));
        }

        public void SaveChanges()
        {
            if (SelectedLocationFood?.Location is null)
            {
                MessageBox.Show("Selected item must have a location.");
                return;
            }
            _db.SaveChanges();
            ReLoadData();
        }

    }
}
