using System.Windows.Controls;

using KnockOff.ViewModels;

namespace KnockOff.Views
{
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
