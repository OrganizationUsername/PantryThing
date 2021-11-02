using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pantry.WPF.Shared
{
    public class DelegateCommand : ICommand
    {
        private readonly Func<Task> _action;

        public DelegateCommand(Func<Task> action)
        {
            _action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            await _action();
        }

        public event EventHandler? CanExecuteChanged;
    }
}
