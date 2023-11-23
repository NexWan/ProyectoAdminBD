using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdminBD.MVVM.Model
{
    internal class ElementosRegistro
    {
        public int _no_oficialia {  get; set; }
        public string _municipio { get; set; }
        public string _nombreOficial {get; set; }
        public string _apPaternoOficial {  get; set; }
        public string _apMaternoOficial { get; set; }
        public string _nombreAsistente {  get; set; }
        public string _apPaternoAsistente { get; set; }
        public string _apMaternoAsistente {get; set; }
    }
}
