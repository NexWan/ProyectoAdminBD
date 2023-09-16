***REMOVED***
***REMOVED***

***REMOVED***.Core
***REMOVED***
    class RelayCommand : ICommand
    ***REMOVED***
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        ***REMOVED***
            add ***REMOVED*** CommandManager.RequerySuggested += value; ***REMOVED***
            remove ***REMOVED*** CommandManager.RequerySuggested -= value; ***REMOVED***
***REMOVED***

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        ***REMOVED***
            _execute = execute;
            _canExecute = canExecute;
***REMOVED***

        public bool CanExecute(object parameter)
        ***REMOVED***
            return _canExecute != null || _canExecute(parameter); 
***REMOVED***

        public void Execute(object parameter)
        ***REMOVED***
            _execute(parameter);
***REMOVED***
***REMOVED***
***REMOVED***
