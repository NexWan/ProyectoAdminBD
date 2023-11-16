using ProyectoAdminBD.Core;


namespace ProyectoAdminBD.MVVM.ViewModel
{
    internal class ViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand {  get; set; }
        public RelayCommand RegViewCommand { get; set; }
        public RelayCommand PresViewCommand { get; set; }
        public RelayCommand PaisViewCommand { get; set; }
        public RelayCommand ParentalesViewCommand {  get; set; }

        public HomeViewModel HomeVm { get; set; }
        public RegisterViewModel RegVm { get; set; }
        public PresentadoViewModel PresentadoVm { get; set; }
        public PaisViewModel PaisVm { get; set; }
        public ParentalesViewModel ParentalesVm { get; set; }


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
            PaisVm = new PaisViewModel();
            ParentalesVm = new ParentalesViewModel();
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

            PaisViewCommand = new RelayCommand(o =>
            {
                CurrentView = PaisVm;
            });

            ParentalesViewCommand = new RelayCommand(o =>
            {
                CurrentView = ParentalesVm;
            });
        }
    }
}
