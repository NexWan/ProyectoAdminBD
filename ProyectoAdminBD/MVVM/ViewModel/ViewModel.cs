using ProyectoAdminBD.Core;


namespace ProyectoAdminBD.MVVM.ViewModel
{
    internal class ViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand {  get; set; }
        public RelayCommand RegViewCommand { get; set; }
        public RelayCommand PresViewCommand { get; set; }

        public HomeViewModel HomeVm { get; set; }
        public RegisterViewModel RegVm { get; set; }
        public PresentadoViewModel PresentadoVm { get; set; }


        private object _currentView;
            
        public object CurrentView
        {
            get { return _currentView; }
            set { 
                _currentView = value; 
                OnPropertyChanged();
            }
        }

        public ViewModel()
        {
            HomeVm = new HomeViewModel();
            RegVm = new RegisterViewModel();
            PresentadoVm = new PresentadoViewModel();
            CurrentView = HomeVm;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVm;
            });

            RegViewCommand = new RelayCommand(o =>
            {
                CurrentView = RegVm;
            });

            PresViewCommand = new RelayCommand(o =>
            {
                CurrentView = PresentadoVm;
            });
        }
    }
}
