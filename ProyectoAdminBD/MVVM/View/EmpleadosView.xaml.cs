using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
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

namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Interaction logic for EmpleadosView.xaml
    /// </summary>
    public partial class EmpleadosView : UserControl
    {
        private IConfiguration _configuration;
        List<ElementosRegistro> ElementosRegistros;
        List<Empleados> listData;
        DataHolder holder;
        SqlDataReader reader;
        SqlCommand cmd;
        bool _clear;
        TextBox[] TextBoxes;
        public EmpleadosView()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            UpdateList();
            NumOficialia.ItemsSource = GetElementos();

        }

        private void UpdateList()
        {
            List<Empleados> list = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM empleados",
                row => new Empleados
                {
                    Id = Convert.ToInt32(row["id_empleado"]),
                    Nombre = row["nombre"].ToString(),
                    ApMaterno = row["ap_materno"].ToString(),
                    ApPaterno = row["ap_paterno"].ToString(),
                    Clave = row["clave"].ToString(),
                    No_oficialia = Convert.ToInt32(row["NO_OFICIALIA"])
                });
            listData = list;
            EmpleadosList.ItemsSource = list;
        }

        private List<ElementosRegistro> GetElementos()
        {
            List<ElementosRegistro> list = new QueryExecutor().ExecuteQuery(
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
            return list;    
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            var id = ID.Text;
            string name = Nombre.Text;
            string apPaterno = ApPaterno.Text;
            string apMaterno = ApMaterno.Text;
            var numOficialia = NumOficialia?.SelectedValue?.ToString();
            string clave = ConvertToUnsecureString(Clave.SecurePassword);
            if (!(holder.CheckForValidText(name) && holder.CheckForValidText(apPaterno) && holder.CheckForValidText(apMaterno)))
            {
                new ShoInfoMsg(ShoInfoMsg.ERROR, "Caracteres ilegales detectados, intente de nuevo");
                return;
            }
            using(SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                conn.Open();
                string q;
                string context = (sender as Button).Content.ToString();
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(numOficialia) && context != "GUARDAR")
                {
                    new ShoInfoMsg(ShoInfoMsg.ERROR, "El campo numero oficialia o Id no puede estar vacio!");
                    return;
                }
                switch (context.ToUpper())
                {
                    case "GUARDAR":
                        if (string.IsNullOrEmpty(numOficialia))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El campo numero oficialia no puede estar vacio!");
                            return;
                        }
                        q = $"SELECT * FROM empleados WHERE id_empleado = {id}";
                        if(VerifyExistingValue(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Dato ya existente en la BD!");
                            return;
                        }
                        q = $"INSERT INTO empleados VALUES ('{id}','{name}','{apPaterno}','{apMaterno}','{clave}','{numOficialia}')";
                        if(ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                        UpdateList();
                        break;
                }
            }
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

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {

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

        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException(nameof(securePassword));

            IntPtr unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
