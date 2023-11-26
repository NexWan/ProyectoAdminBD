using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Interaction logic for PersonaRegistradaView.xaml
    /// </summary>
    public partial class PersonaRegistradaView : UserControl
    {
        private IConfiguration _configuration;
        List<ElementosRegistro> elementosRegistros;
        List<Municipio> municipios;
        List<Presentado> presentados;
        List<PersonaRegistrada> listData;
        List<Padres> padres;
        DataHolder holder;
        SqlDataReader reader;
        SqlCommand cmd;
        bool _clear;
        TextBox[] TextBoxes;
        public PersonaRegistradaView()
        {
            InitializeComponent();
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            holder = DataHolder.Instance;
            MUNICIPIO.ItemsSource = GetMunicipios();
            GENERO.ItemsSource = GetGeneros();
            PRESENTADO.ItemsSource = GetPresentados();
            OFICIALIA.ItemsSource = GetElementosRegistros();
            FECHANAC.DisplayDate = DateTime.Now;
            FECHAREG.DisplayDate = DateTime.Now;
            UpdateList();
        }

        private void UpdateList()
        {
            List<PersonaRegistrada> list = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM persona_registrada",
                row => new PersonaRegistrada
                {
                    Curp = row["curp"].ToString(),
                    Municipio = row["idMunicipio"].ToString(),
                    Genero = row["id_Genero"].ToString(),
                    Presentado = row["idPRESENTADO"].ToString(),
                    No_oficialia = Convert.ToInt32(row["NO_OFICIALIA"]),
                    Nombre = row["NOMBRES"].ToString(),
                    ApPaterno = row["APELLIDO_PATERNO"].ToString(),
                    ApMaterno = row["APELLIDO_MATERNO"].ToString(),
                    FechaNac = (DateTime)row["FECHA_NACIMIENTO"],
                    HoraNac = (TimeSpan)row["HORA_NAC"],
                    Crip = row["CRIP"].ToString(),
                    No_acta = Convert.ToInt32(row["NO_ACTA"]),
                    No_libro = Convert.ToInt32(row["NO_LIBRO"]),
                    FechaReg = (DateTime)row["FECHA_REGISTRO"],
                    CurpPadre = row["padre_curp"].ToString(),
                    CurpMadre = row["madre_curp"].ToString(),
                });
            ActaTable.ItemsSource = list;
            listData = list;
        }

        private List<Municipio> GetMunicipios()
        {
            List<Municipio> temp =  new QueryExecutor().ExecuteQuery(
                "SELECT * FROM municipio",
                row => new Municipio
                {
                    _id = row["id_municipio"].ToString(),
                    _entidad = row["id_entidad"].ToString(),
                    _name = row["nombre"].ToString()
                }
                );
            municipios = temp;
            return municipios;
        }

        private List<Genero> GetGeneros()
        {
            return new QueryExecutor().ExecuteQuery(
                "SELECT * FROM genero",
                row => new Genero
                {
                    Id = row["Id_GENERO"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                });
        }

        private List<Presentado> GetPresentados()
        {
            return new QueryExecutor().ExecuteQuery(
                "SELECT * FROM presentado",
                row => new Presentado
                {
                    _id = row["id_PRESENTADO"].ToString(),
                    _descripcion = row["Descripcion"].ToString()
                });
        }

        private List<ElementosRegistro> GetElementosRegistros()
        {
            return new QueryExecutor().ExecuteQuery(
                "SELECT * FROM elementos_registro",
                row => new ElementosRegistro
                {
                    _no_oficialia = Convert.ToInt32(row["NO_OFICIALIA"]),
                    _municipio = row["id_Municipio"].ToString(),
                    _nombreOficial = row["nombreOficialMayor"].ToString(),
                    _apPaternoOficial = row["apPaternoOficialMayor"].ToString(),
                    _apMaternoOficial = row["apMaternoOficialMayor"].ToString(),
                    _nombreAsistente = row["nombreAsienta"].ToString(),
                    _apMaternoAsistente = row["apMaternoAsienta"].ToString(),
                    _apPaternoAsistente = row["apPaternoAsienta"].ToString()
                });
        }

        private void DisableButtons()
        {
            Save.IsEnabled = false;
            Update.IsEnabled = false;
            Delete.IsEnabled = false;
            Clear.IsEnabled = false;
        }

        public void EnableButtons()
        {
            Save.IsEnabled = true;
            Update.IsEnabled = true;
            Delete.IsEnabled = true;
            Clear.IsEnabled = true;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            string curp = CURP.Text;
            string municipio = MUNICIPIO?.SelectedValue.ToString();
            string? genero = GENERO?.SelectedValue?.ToString();
            string? presentado = PRESENTADO?.SelectedValue?.ToString();
            string? oficialia = OFICIALIA?.SelectedValue?.ToString();
            string nombre = NOMBRE.Text;
            string apPaterno = APPATERNO.Text;
            string apMaterno = APMATERNO.Text;
            string curpPadre = CURPPADRE.Text;
            string curpMadre = CURPMADRE.Text;
            DateTime? fechaNac = FECHANAC.SelectedDate;
            TimeSpan? horaNac = TimeSpan.Parse(HORANAC.Text);
            string crip = CRIP.Text;
            string numActa = NUMACTA.Text;
            string numLibro = NUMLIBRO.Text;
            DateTime? fechaReg = FECHAREG.SelectedDate;
            Debug.WriteLine($"CURP: {curp}");
            Debug.WriteLine($"Municipio: {municipio}");
            Debug.WriteLine($"Genero: {genero}");
            Debug.WriteLine($"Presentado: {presentado}");
            Debug.WriteLine($"Oficialia: {oficialia}");
            Debug.WriteLine($"Nombre: {nombre}");
            Debug.WriteLine($"Apellido Paterno: {apPaterno}");
            Debug.WriteLine($"Apellido Materno: {apMaterno}");
            Debug.WriteLine($"CURP Padre: {curpPadre}");
            Debug.WriteLine($"CURP Madre: {curpMadre}");
            Debug.WriteLine($"Fecha de Nacimiento: {fechaNac}");
            Debug.WriteLine($"Hora de Nacimiento: {horaNac}");
            Debug.WriteLine($"CRIP: {crip}");
            Debug.WriteLine($"Número de Acta: {numActa}");
            Debug.WriteLine($"Número de Libro: {numLibro}");
            Debug.WriteLine($"Fecha de Registro: {fechaReg}");
            string context = (sender as Button).Content.ToString().ToUpper();
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                conn.Open();
                string q;
                switch (context)
                {
                    case "GUARDAR":
                        if(!string.IsNullOrEmpty(curp))
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "Se ha detectado que el campo curp contiene texto, este sera sobreescrito al generado automaticamente");
                        
                        if (string.IsNullOrEmpty(municipio) ||
                            string.IsNullOrEmpty(genero) || string.IsNullOrEmpty(presentado) ||
                            string.IsNullOrEmpty(oficialia) || string.IsNullOrEmpty(nombre) ||
                            string.IsNullOrEmpty(apPaterno) || string.IsNullOrEmpty(apMaterno) ||
                            string.IsNullOrEmpty(curpPadre) || string.IsNullOrEmpty(curpMadre) ||
                            fechaNac == null || horaNac == null ||
                            string.IsNullOrEmpty(crip) || string.IsNullOrEmpty(numActa) ||
                            string.IsNullOrEmpty(numLibro) || fechaReg == null)
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "1 o mas campos vacios, por favor intente de nuevo!");
                            return;
                        }
                        string nuCurp = GenerarCurp(apPaterno, apMaterno, nombre, fechaNac, genero, municipio);
                        Debug.WriteLine(nuCurp);
                        q = $"SELECT * FROM persona_registrada where curp = '{nuCurp}'";
                        if (VerifyExistingValue(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Dato ya existe en la BD! intente de nuevo");
                            return;
                        }
                        if(!VerifyExistingValue(conn, $"SELECT * FROM padre WHERE curp = '{curpPadre}'"))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR,"Curp de padre no existente!");
                            return;
                        }
                        else if(!VerifyExistingValue(conn,$"SELECT * FROM madre WHERE curp = '{curpMadre}'"))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Curp de madre no existente!");
                            return;
                        }
                        q = $"INSERT INTO persona_registrada VALUES (" +
                                $"'{nombre}', " +
                                $"'{apMaterno}', " +
                                $"'{apPaterno}', " +
                                $"'{fechaNac?.ToString("yyyy-MM-dd")}', " + // Format the date for SQL
                                $"'{horaNac?.ToString()}', " + // Use the default TimeSpan.ToString() format
                                $"'{crip}', " +
                                $"'{nuCurp}', " +
                                $"'{curpMadre}', " +
                                $"'{curpPadre}', " +
                                $"'{municipio}', " +
                                $"'{genero}', " +
                                $"'{presentado}', " +
                                $"{oficialia}," +
                                $"{numActa}, " +
                                $"{numLibro}," +
                                $"'{fechaReg?.ToString("yyyy-MM-dd")}')";
                        if(ExecQuery(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                        }
                        break;
                }
            }
        }

        private string GenerarCurp(string apPaterno, string apMaterno, string nombre, DateTime? fechaNac, string genero, string municipio)
        {
            Random rnd = new Random();
            string iniciales = $"{apPaterno[0]}";

            for (int i = 1; i < apPaterno.Length; i++)
            {
                if ("AEIOU".Contains(apPaterno[i], StringComparison.OrdinalIgnoreCase))
                {
                    iniciales += char.ToUpper(apPaterno[i]);
                    break;
                }
            }

            string estado = municipios.FirstOrDefault(m => m._id == municipio)._entidad;

            iniciales += $"{apMaterno[0]}{nombre[0]}".ToUpper();

            // Obtener los dígitos del año de nacimiento
            string anioNac = (fechaNac?.Year % 100)?.ToString("D2");

            // Obtener el carácter del mes de nacimiento (en el caso de enero, se utiliza "0")
            string mesNac = fechaNac?.Month == 1 ? "0" : fechaNac?.Month.ToString("D2");

            // Obtener los dígitos del día de nacimiento
            string diaNac = fechaNac?.Day.ToString("D2");
            string consonantes = $"{ObtenerConsonante(apPaterno.Substring(1))}{ObtenerConsonante(apMaterno)}{ObtenerConsonante(nombre)}";

            // Construir el string de la CURP
            return $"{iniciales}{anioNac}{mesNac}{diaNac}{genero[0]}{estado.Substring(0,2)}{consonantes}{iniciales[0]}{rnd.NextInt64(10,99)}";
        }

        private char ObtenerConsonante(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!EsVocal(s[i]))
                {
                    return char.ToUpper(s[i]);
                }
            }
            return '\0';
        }

        bool EsVocal(char letra)
        {
            return "AEIOUaeiou".Contains(letra);
        }


        private bool ExecQuery(SqlConnection conn, string query)
        {
            Debug.WriteLine(query);
            if (reader != null) reader.Close();
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            if (cmd.ExecuteNonQuery() > 0)
            {
                return true;
            }
            return false;
        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {

        }

        private bool VerifyExistingValue(SqlConnection conn, string query)
        {
            if (reader != null) reader.Close();
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            return false;
        }

    }
}
