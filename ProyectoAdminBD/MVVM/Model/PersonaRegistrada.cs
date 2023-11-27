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
        public string Padre { get; set; }
        public string Madre { get; set; }
        public string CurpPadre { get; set; }
        public string CurpMadre { get; set; }
        public string AbueloPaterno { get; set; }
        public string AbueloMaterno { get; set; }
        public int AbueloPaternoId { get; set; }
        public int AbuelaPaternaId { get; set; }
        public int AbueloMaternoId { get; set; }
        public int AbuelaMaternaId { get; set; }
    }
}
