using System.Linq;
using System.Threading.Tasks;
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

        public async Task LoadData(int mealInstanceId)
        {

            SelectedMealInstance = await _itemService.GetMealInstance(mealInstanceId);
        }
    }
}
