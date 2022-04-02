using System.Windows.Controls;

using KnockOff.ViewModels;

namespace KnockOff.Views
{
    public partial class ContentGridDetailPage : Page
    {
        public ContentGridDetailPage(ContentGridDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
