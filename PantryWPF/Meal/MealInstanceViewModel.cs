using System;
using System.Linq;
using Pantry.Core.Models;
using Pantry.ServiceGateways;
using Stylet;

namespace Pantry.WPF.Meal
{
    public class MealInstanceViewModel : Screen
    {
        private readonly ItemService _itemService;
        private readonly Func<MealInstanceDetailViewModel> _mealInstanceDetailVmFactory;
        private MealInstance _selectedMealInstance;
        public BindableCollection<MealInstance> MealInstances { get; set; } = new();
        public MealInstanceDetailViewModel SelectedDetailViewModel { get; set; }
        public MealInstance SelectedMealInstance
        {
            get => _selectedMealInstance;
            set
            {
                SetAndNotify(ref _selectedMealInstance, value, nameof(SelectedMealInstance));
                if (_selectedMealInstance is not null)
                {
                    SelectedDetailViewModel = _mealInstanceDetailVmFactory();
                    SelectedDetailViewModel.LoadData(_selectedMealInstance.MealInstanceId);
                    OnPropertyChanged(nameof(SelectedDetailViewModel));
                }
            }
        }

        public MealInstanceViewModel(ItemService itemService, Func<MealInstanceDetailViewModel> mealInstanceDetailVmFactory)
        {
            _itemService = itemService;
            _mealInstanceDetailVmFactory = mealInstanceDetailVmFactory;
            LoadData();
        }

        public void LoadData()
        {
            MealInstances.Clear();
            MealInstances.AddRange(
                _itemService
                .GetMealInstances()
                .OrderBy(x => x.DaysSinceZero)
                .ThenBy(x => x.MealInstanceDateTime));
        }
    }
}
