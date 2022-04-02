using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using KnockOff.Contracts.ViewModels;

using KnockOff.CoreLogic.Contracts.Services;
using KnockOff.CoreLogic.Models;
using KnockOff.Helpers;


namespace KnockOff.ViewModels
{
    public class ListDetailsViewModel : ObservableObject, INavigationAware
    {
        private readonly ISampleDataService _sampleDataService;
        private SampleOrder _selected;

        public SampleOrder Selected { get => _selected; set => SetProperty(ref _selected, value); }

        public ObservableCollection<SampleOrder> SampleItems { get; } = new();

        public ListDetailsViewModel(ISampleDataService sampleDataService) => _sampleDataService = sampleDataService;

        public async void OnNavigatedTo(object parameter)
        {
            SampleItems.Replace(await _sampleDataService.GetListDetailsDataAsync());
            Selected = SampleItems.First();
        }

        public void OnNavigatedFrom() { }
    }
}
