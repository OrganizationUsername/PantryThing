using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Pantry.Core.Models;
using Pantry.Data;
using PantryWPF.Annotations;

namespace PantryWPF.Recipes
{
    public class RecipeDetailViewModel : Recipe, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Pantry.Core.Models.Food> Foods { get; set; }

        public FakeSaveCommand FakeSaveCommand { get; set; }
        public RecipeDetailViewModel()
        {
            var dataBase = new DataBase();
            Foods = dataBase.Foods.ToList();
            FakeSaveCommand = new FakeSaveCommand(this);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}