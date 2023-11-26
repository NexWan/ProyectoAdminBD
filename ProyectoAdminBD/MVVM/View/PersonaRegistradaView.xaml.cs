using Microsoft.Extensions.Configuration;
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
            return new QueryExecutor().ExecuteQuery(
                "SELECT * FROM municipio",
                row => new Municipio
                {
                    _id = row["id_municipio"].ToString(),
                    _entidad = row["id_entidad"].ToString(),
                    _name = row["nombre"].ToString()
                }
                );
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

        private void HORANAC_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string currentText = textBox.Text;

            // Remove any existing colons from the text
            currentText = currentText.Replace(":", "");

            // Insert colons every 2 characters
            if (currentText.Length > 1)
            {
                currentText = currentText.Insert(2, ":");
            }

            // Set the formatted text back to the TextBox
            textBox.Text = currentText;      
        } 
    }
}
