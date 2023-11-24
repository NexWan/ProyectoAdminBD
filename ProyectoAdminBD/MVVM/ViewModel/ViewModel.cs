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
        public RelayCommand EntidadViewCommand { get; set; }
        public RelayCommand MunicipioViewCommand { get; set; }
        public RelayCommand ElRegistroViewCommand {  get; set; }

        public HomeViewModel HomeVm { get; set; }
        public RegisterViewModel RegVm { get; set; }
        public PresentadoViewModel PresentadoVm { get; set; }
        public PaisViewModel PaisVm { get; set; }
        public ParentalesViewModel ParentalesVm { get; set; }
        public EntidadViewModel EntidadVm { get; set; }
        public MunicipioViewModel MunicipioVm { get; set; }
        public ElRegistroViewModel ElRegistroVm { get; set; }

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
            EntidadVm = new EntidadViewModel();
            MunicipioVm = new MunicipioViewModel();
            ElRegistroVm = new ElRegistroViewModel();
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

            EntidadViewCommand = new RelayCommand(o =>
            {
                CurrentView = EntidadVm;
            });

            MunicipioViewCommand = new RelayCommand(o =>
            {
                CurrentView = MunicipioVm;
            });

            ElRegistroViewCommand = new RelayCommand(o => 
            { 
                CurrentView = ElRegistroVm; 
            });
        }
    }
}
