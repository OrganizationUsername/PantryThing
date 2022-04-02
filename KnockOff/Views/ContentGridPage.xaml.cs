using System.Windows.Controls;

using KnockOff.ViewModels;

namespace KnockOff.Views
{
    public partial class ContentGridPage : Page
    {
        public ContentGridPage(ContentGridViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
