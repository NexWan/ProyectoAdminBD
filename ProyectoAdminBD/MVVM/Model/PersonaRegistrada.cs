using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdminBD.MVVM.Model
{
    class PersonaRegistrada
    {
        public string Curp { get; set; }
        public string Municipio { get; set; }
        public string Genero { get; set; }
        public string Presentado { get; set; }
        public int No_oficialia { get; set; }
        public string Nombre { get; set; }
        public string ApPaterno { get; set; }
        public string ApMaterno { get; set; }
        public DateTime FechaNac {  get; set; }
        public TimeSpan HoraNac { get; set; }
        public string Crip {  get; set; }
        public int No_acta {  get; set; }
        public int No_libro { get; set; }
        public DateTime FechaReg {  get; set; }
        public String CurpPadre { get; set; }
        public string CurpMadre { get; set; }
    }
}
