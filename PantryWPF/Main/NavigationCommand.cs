using System;
using System.Windows.Input;
using Pantry.WPF.Shared;
using Stylet;

namespace Pantry.WPF.Main
{
    public class NavigationCommand : ICommand
    {
        private readonly RootViewModel _vm;
        private readonly IScreen _screen;

        public NavigationCommand(RootViewModel vm, IScreen screen)
        {
            _vm = vm;
            _screen = screen;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _vm.ActivateItem(_screen);
            _vm.VmName = _screen.GetType().ToString();
        }

        public event EventHandler CanExecuteChanged;
    }
}