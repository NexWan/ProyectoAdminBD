using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdminBD.MVVM.Model
{
    class Padres
    {
        public string _curp {  get; set; }
        public string _pais { get; set; }
        public string _nombres { get; set; }
        public string _ap_paterno { get; set; }
        public string _ap_materno { get; set; }
        public int _edad {  get; set; }
        public string _parentezco {  get; set; }
    }
}
