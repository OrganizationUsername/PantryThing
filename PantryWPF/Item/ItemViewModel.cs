using System;
using System.Collections.ObjectModel;
using System.Windows;
using Pantry.ServiceGateways;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Item
{
    public class ItemViewModel : Screen
    {
        private Pantry.Core.Models.Item _selectedItem;
        private Pantry.Core.Models.Food _selectedFood;

        public BindableCollection<Pantry.Core.Models.Item> Items { get; set; }
        public BindableCollection<Pantry.Core.Models.Food> Foods { get; set; }

        public DelegateCommand AddItemCommand { get; set; }
        public DelegateCommand AddLocationFoodCommand { get; set; }

        private readonly ItemService _itemService;

        private string _newItemUpc;
        public string NewItemUpc
        {
            get => _newItemUpc;
            set => SetAndNotify(ref _newItemUpc, value, nameof(NewItemUpc));
        }

        private double _newItemWeight;
        public double NewItemWeight
        {
            get => _newItemWeight;
            set => SetAndNotify(ref _newItemWeight, value, nameof(NewItemWeight));
        }

        public Pantry.Core.Models.Item SelectedItem
        {
            get => _selectedItem;
            set => SetAndNotify(ref _selectedItem, value, nameof(SelectedItem));
        }

        public Pantry.Core.Models.Food SelectedFood
        {
            get => _selectedFood;
            set => SetAndNotify(ref _selectedFood, value, nameof(SelectedFood));
        }


        public ItemViewModel(ItemService itemService)
        {
            _itemService = itemService;
            AddItemCommand = new(AddItem);
            AddLocationFoodCommand = new(AddLocationFood);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadData();
        }


        public void AddLocationFood()
        {
            _itemService.AddLocationFood(SelectedItem);
        }

        public void AddItem()
        {
            if (string.IsNullOrEmpty(NewItemUpc) || SelectedFood is null) { return; }
            bool addItemSuccess = _itemService.AddItem(SelectedFood, NewItemUpc, NewItemWeight);
            if (addItemSuccess)
            {
                LoadData();
            }
            else
            {
                MessageBox.Show("Issue with saving Item.");
            }
        }

        public void LoadData()
        {
            Items = new(_itemService.GetItems());
            Foods = new(_itemService.GetFoods());
        }
    }
}
