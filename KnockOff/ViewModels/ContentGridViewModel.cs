using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KnockOff.Contracts.Services;
using KnockOff.Contracts.ViewModels;
using KnockOff.CoreLogic.Contracts.Services;
using KnockOff.CoreLogic.Models;
using KnockOff.Helpers;

namespace KnockOff.ViewModels
{
    public class ContentGridViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly ISampleDataService _sampleDataService;
        private ICommand _navigateToDetailCommand;

        public ICommand NavigateToDetailCommand { get => _navigateToDetailCommand = _navigateToDetailCommand ?? new RelayCommand<SampleOrder>(NavigateToDetail); }

        public ObservableCollection<SampleOrder> Source { get; } = new();

        public ContentGridViewModel(ISampleDataService sampleDataService, INavigationService navigationService)
        {
            _sampleDataService = sampleDataService;
            _navigationService = navigationService;
        }

        public async void OnNavigatedTo(object parameter) => Source.Replace(await _sampleDataService.GetContentGridDataAsync());

        public void OnNavigatedFrom() => _ = (byte)1;

        private void NavigateToDetail(SampleOrder order)
        {
            if (order is null) { return; }
            _navigationService.NavigateTo(typeof(ContentGridDetailViewModel).FullName, order.OrderID);
        }
    }
}
