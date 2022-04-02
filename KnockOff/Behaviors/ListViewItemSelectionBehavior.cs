using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KnockOff.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace KnockOff.Behaviors
{
    public class ListViewItemSelectionBehavior : Behavior<ListView>
    {
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ListViewItemSelectionBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            var listView = AssociatedObject as ListView;
            listView.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            listView.KeyDown += OnKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            var listView = AssociatedObject as ListView;
            listView.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            listView.KeyDown -= OnKeyDown;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Command != null && e.OriginalSource is FrameworkElement selectedItem)
            {
                var dc = selectedItem.DataContext;
                if (dc is not ContentGridViewModel /* Otherwise it fires when whitespace inside ListView is clicked. */)
                {
                    SelectItem(e);
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            SelectItem(e);
            e.Handled = true;
        }

        private void SelectItem(RoutedEventArgs args)
        {
            if (Command != null && args.OriginalSource is FrameworkElement selectedItem && Command.CanExecute(selectedItem.DataContext))
            {
                Command.Execute(selectedItem.DataContext);
            }
        }
    }
}
