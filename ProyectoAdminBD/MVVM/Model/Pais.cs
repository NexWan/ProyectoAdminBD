
namespace ProyectoAdminBD.MVVM.Model
{
     class Pais
    {
        public string _id { get; set; }
        public string _nombre { get; set; }
        public string _nacionalidad { get; set; }

        public override string ToString()
        {
            return _nombre;
        }
    }
}
