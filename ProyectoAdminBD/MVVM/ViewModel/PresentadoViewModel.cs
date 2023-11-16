using ProyectoAdminBD.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdminBD.MVVM.ViewModel
{
    class PresentadoViewModel
    {
        public ObservableCollection<Pais> paisManager {  get; set; }
    }
}
