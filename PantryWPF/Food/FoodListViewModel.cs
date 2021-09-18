using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pantry.Data;
using PantryWPF.Main;

namespace PantryWPF.Food
{
    public class FoodListViewModel : VmBase
    {
        private readonly IDataBase _dataBase;
        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; }

        public FoodListViewModel()
        {
            _dataBase = new DataBase();
            _dataBase.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Bread" });
            Foods = new ObservableCollection<Pantry.Core.Models.Food>(_dataBase.Foods.ToList());
        }


    }
}
