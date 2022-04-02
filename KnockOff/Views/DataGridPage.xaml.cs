using System.Windows.Controls;

using KnockOff.ViewModels;

namespace KnockOff.Views
{
    public partial class DataGridPage : Page
    {
        public DataGridPage(DataGridViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
