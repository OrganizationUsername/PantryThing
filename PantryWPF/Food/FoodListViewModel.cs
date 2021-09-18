using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pantry.Data;

namespace PantryWPF.Food
{
    public class FoodListViewModel
    {
        private readonly IDataBase _dataBase;
        public ObservableCollection<Pantry.Core.Models.Food> Foods { get; set; }

        public FoodListViewModel(IDataBase dataBase)
        {
            _dataBase = dataBase;
            dataBase.Foods.Add(new Pantry.Core.Models.Food() { FoodName = "Bread" });
            Foods = new ObservableCollection<Pantry.Core.Models.Food>(dataBase.Foods.ToList());
        }


    }
}
