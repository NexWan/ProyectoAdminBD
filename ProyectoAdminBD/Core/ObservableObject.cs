***REMOVED***
using System.ComponentModel;
using System.Runtime.CompilerServices;

***REMOVED***.Core
***REMOVED***
    class ObservableObject : INotifyPropertyChanged
    ***REMOVED***
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPorpertyChanged([CallerMemberName] string name = null)
        ***REMOVED***
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
***REMOVED***
***REMOVED***
***REMOVED***
