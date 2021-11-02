using System.Linq;
using Pantry.Core.Models;
using Pantry.ServiceGateways;
using Stylet;

namespace Pantry.WPF.Meal
{
    public class MealInstanceDetailViewModel
    {
        private readonly ItemService _itemService;
        public BindableCollection<MealInstance> MealInstances { get; set; } = new();
        public MealInstance SelectedMealInstance { get; set; }

        public MealInstanceDetailViewModel(ItemService itemService)
        {
            _itemService = itemService;

        }

        public void LoadData(int mealInstanceId)
        {

            SelectedMealInstance = _itemService.GetMealInstance(mealInstanceId);
        }
    }
}
