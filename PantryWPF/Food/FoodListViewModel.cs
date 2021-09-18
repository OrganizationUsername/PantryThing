using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Food
{
    public class FoodListViewModel : VmBase
    {
        private readonly IDataBase _dataBase;
        private Pantry.Core.Models.Food _selectedFood;
        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; }
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
            if (Foods is null || Foods.Count == 0)
            {
                Foods = new ObservableCollection<Pantry.Core.Models.Food>(_dataBase.Foods.ToList());
                OnPropertyChanged(nameof(Foods));
                return;
            }

            for (var index = 0; index < Foods.Count; index++)
            {
                var x = Foods[index];
                Foods.Remove(x);
            }

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
