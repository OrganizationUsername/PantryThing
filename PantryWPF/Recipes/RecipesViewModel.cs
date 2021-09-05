using System;
using System.Collections.ObjectModel;
using PantryWPF.Main;

namespace PantryWPF.Recipes
{
    public class RecipesViewModel : VmBase
    {
        public ObservableCollection<string> ACollection { get; set; }
        public RecipesViewModel()
        {
            ACollection = new();
            ACollection.Add("asdf1");
            ACollection.Add("asdf2");
            ACollection.Add("asdf3");
            ACollection.Add($"asdf4{DateTime.Now}");
        }
    }
}
