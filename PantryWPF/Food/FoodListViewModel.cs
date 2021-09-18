using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Food
{
    public class FoodListViewModel : VmBase
    {
        private readonly IDataBase _dataBase;
        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; }

        public DelegateCommand AddRecipeCommand { get; set; }
        public FoodListViewModel()
        {
            _dataBase = new DataBase();
            LoadFoods();
            AddRecipeCommand = new DelegateCommand(AddFood);
        }

        public void LoadFoods()
        {
            if (Foods is null || Foods.Count == 0)
            {
                Foods = new ObservableCollection<Pantry.Core.Models.Food>(_dataBase.Foods.ToList());
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
        }

        public void AddFood()
        {
            _dataBase.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Something. " });
            _dataBase.SaveChanges();
        }

    }




}
