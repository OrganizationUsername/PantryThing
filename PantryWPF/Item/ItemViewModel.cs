using System.Collections.ObjectModel;
using System.Windows;
using PantryWPF.Main;
using ServiceGateways;

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
        private readonly ItemService _itemService;
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
            _itemService = new(); //This should be injected.
            AddItemCommand = new(AddItem);
            AddLocationFoodCommand = new(AddLocationFood);
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
                OnPropertyChanged(nameof(SelectedFood));
                OnPropertyChanged(nameof(NewItemUpc));
                OnPropertyChanged(nameof(NewItemWeight));
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
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(Foods));
        }

    }


}
