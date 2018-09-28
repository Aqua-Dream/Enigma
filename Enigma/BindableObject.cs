using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Enigma
{
    public abstract class BindableObject:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if(propertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void SetProperty<T>(ref T item, T value, [CallerMemberName] string propertyName=null)
        {
            item = value;
            OnPropertyChanged(propertyName);
        }
    }
}
