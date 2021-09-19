using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Food
{
    public class FoodListViewModel : VmBase
    {
        private readonly DataBase _dataBase;
        private Pantry.Core.Models.Food _selectedFood;

        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; } = new ObservableCollection<Pantry.Core.Models.Food>();
        public string NewFoodName { get; set; }
        public Pantry.Core.Models.Food SelectedFood
        {
            get => _selectedFood;
            set { _selectedFood = value; OnPropertyChanged(nameof(SelectedFood)); }
        }

        public DelegateCommand AddRecipeCommand { get; set; }
        public FoodListViewModel()
        {
            _dataBase = new DataBase();
            KeepOnlyUniqueFoodNames();
            LoadFoods();
            AddRecipeCommand = new DelegateCommand(AddFood);
        }

        public void KeepOnlyUniqueFoodNames()
        {
            var Names = new List<string>();
            foreach (var x in _dataBase.Foods)
            {
                if (!Names.Contains(x.FoodName) && !string.IsNullOrWhiteSpace(x.FoodName))
                {
                    Names.Add(x.FoodName);
                }
                else
                {
                    _dataBase.Foods.Remove(x);
                }
            }
            _dataBase.SaveChanges();
        }


        public void LoadFoods()
        {

            if (_dataBase.Foods is null )
            {
                Foods = new ObservableCollection<Pantry.Core.Models.Food>();
                OnPropertyChanged(nameof(Foods));
                return;
            }

            Foods.Clear();

            foreach (var x in _dataBase.Foods)
            {
                Foods.Add(x);
            }
            SelectedFood = Foods.First();
        }

        public void AddFood()
        {
            if (string.IsNullOrWhiteSpace(NewFoodName)) { return; }
            _dataBase.Foods.Add(new Pantry.Core.Models.Food() { FoodName = NewFoodName });
            _dataBase.SaveChanges();
            NewFoodName = "";
            OnPropertyChanged(nameof(NewFoodName));
            LoadFoods();
            SelectedFood = Foods.Last();
        }

    }
}
