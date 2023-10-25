using ProyectoAdminBD.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdminBD.MVVM.ViewModel
{
    internal class ViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand {  get; set; }
        public RelayCommand RegViewCommand { get; set; }

        public HomeViewModel HomeVm { get; set; }
        public RegisterViewModel RegVm { get; set; }

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
            CurrentView = HomeVm;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVm;
            });

            RegViewCommand = new RelayCommand(o =>
            {
                CurrentView = RegVm;
            });
        }
    }
}
