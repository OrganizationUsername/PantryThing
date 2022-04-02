using System.Windows.Controls;

using KnockOff.Contracts.Views;
using KnockOff.ViewModels;

using MahApps.Metro.Controls;

namespace KnockOff.Views
{
    public partial class ShellWindow : MetroWindow, IShellWindow
    {
        public ShellWindow(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public Frame GetNavigationFrame() => shellFrame;
        public void ShowWindow() => Show();
        public void CloseWindow() => Close();
    }
}
