using System.Windows.Controls;

using KnockOff.ViewModels;

namespace KnockOff.Views
{
    public partial class ListDetailsPage : Page
    {
        public ListDetailsPage(ListDetailsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
