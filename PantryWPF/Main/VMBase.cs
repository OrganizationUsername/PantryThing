using System.ComponentModel;
using System.Runtime.CompilerServices;
using PantryWPF.Annotations;

namespace PantryWPF.Main
{
    public class VmBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}