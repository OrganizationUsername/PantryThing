using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using KnockOff.Contracts.ViewModels;
using KnockOff.CoreLogic.Contracts.Services;
using KnockOff.CoreLogic.Models;
using KnockOff.Helpers;

namespace KnockOff.ViewModels
{
    public class DataGridViewModel : ObservableObject, INavigationAware
    {
        private readonly ISampleDataService _sampleDataService;

        public ObservableCollection<SampleOrder> Source { get; } = new();

        public DataGridViewModel(ISampleDataService sampleDataService) => _sampleDataService = sampleDataService;

        public async void OnNavigatedTo(object parameter) => Source.Replace(await _sampleDataService.GetGridDataAsync());

        public void OnNavigatedFrom() { }
    }
}
