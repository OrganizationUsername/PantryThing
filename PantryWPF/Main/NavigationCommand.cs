using System;
using System.Windows.Input;

namespace PantryWPF.Main
{
    public class NavigationCommand : ICommand
    {
        private readonly MainWindowViewModel _vm;
        private readonly VmBase _vmBase;

        public NavigationCommand(MainWindowViewModel vm, VmBase vmBase)
        {
            _vm = vm;
            _vmBase = vmBase;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _vm.MainView = _vmBase;
            _vm.VmName = _vmBase.GetType().ToString();
            _vm.OnPropertyChanged(nameof(_vm.VmName));
            _vm.OnPropertyChanged(nameof(_vm.MainView));
        }

        public event EventHandler CanExecuteChanged;
    }
}