using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Item
{
    public class ItemViewModel : VmBase
    {
        private Pantry.Core.Models.Item _selectedItem;
        private Pantry.Core.Models.Food _selectedFood;

        public ObservableCollection<Pantry.Core.Models.Item> Items { get; set; }
        public DelegateCommand AddItemCommand { get; set; }
        public DelegateCommand AddLocationFoodCommand { get; set; }
        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; }
        public string NewItemUpc { get; set; }
        public double NewItemWeight { get; set; }
        public Pantry.Core.Models.Item SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }


        public Pantry.Core.Models.Food SelectedFood
        {
            get => _selectedFood;
            set { _selectedFood = value; OnPropertyChanged(nameof(SelectedFood)); }
        }


        public ItemViewModel()
        {
            AddItemCommand = new(AddItem);
            AddLocationFoodCommand = new(AddLocationFood);
            LoadData();
        }


        public void AddLocationFood()
        {
            using (var db = new DataBase())
            {
                db.LocationFoods.Add(new()
                {
                    Exists = true,
                    ExpiryDate = DateTime.MinValue,
                    ItemId = SelectedItem.ItemId,
                    Location = db.Locations.First(),
                    OpenDate = DateTime.MinValue,
                    Quantity = SelectedItem.Weight
                });
                db.SaveChanges();
            }
        }

        public void AddItem()
        {
            if (string.IsNullOrEmpty(NewItemUpc) || SelectedFood is null) { return; }

            using (var db = new DataBase())
            {
                if (db.Items.Any(x => x.Upc == "")) { return; }

                db.Items.Add(new() { FoodId = SelectedFood.FoodId, Unit = null, Upc = NewItemUpc, Weight = NewItemWeight });
                db.SaveChanges();
                SelectedFood = null;
                NewItemUpc = "";
                NewItemWeight = 0;
                OnPropertyChanged(nameof(SelectedFood));
                OnPropertyChanged(nameof(NewItemUpc));
                OnPropertyChanged(nameof(NewItemWeight));
            }

            LoadData();

        }

        public void LoadData()
        {
            using (var db = new DataBase())
            {
                Items = new(db.Items.Include(x => x.Food).ToList());
                Foods = new(db.Foods.ToList());
            }
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(Foods));
        }


    }


}
