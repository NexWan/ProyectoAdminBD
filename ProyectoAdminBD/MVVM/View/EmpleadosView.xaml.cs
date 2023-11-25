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
        bool _clear = true, wasTriggered = false;
        TextBox[] TextBoxes;
        public EmpleadosView()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            UpdateList();
            NumOficialia.ItemsSource = GetElementos();
            TextBoxes = new TextBox[] { ID, Nombre, ApPaterno, ApMaterno };
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
            DisableButtons();
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
            ElementosRegistros = list;
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
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                Button bt = new Button();
                bt.Content = "asasa";
                ShowPass(bt, e);
                conn.Open();
                string q;
                string context = (sender as Button).Content.ToString();
                if (context.ToUpper() == "LIMPIAR")
                {
                    UpdateList();
                    Clean();
                    return;
                }
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
                        if (VerifyExistingValue(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Dato ya existente en la BD!");
                            return;
                        }
                        q = $"INSERT INTO empleados VALUES ('{id}','{name}','{apPaterno}','{apMaterno}','{clave}','{numOficialia}')";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                        UpdateList();
                        Clean();
                        break;
                    case "MODIFICAR":
                        q = $"SELECT * FROM empleados WHERE id_empleado = {id}";
                        if (!VerifyExistingValue(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El usuario no existe en la BD!, intente de nuevo");
                            return;
                        }
                        q = $"UPDATE empleados SET no_oficialia = {numOficialia}, nombre = '{name}', ap_materno = '{apMaterno}'," +
                            $"ap_paterno = '{apMaterno}', clave = '{clave}' WHERE id_empleado = {id}";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                        UpdateList();
                        Clean();
                        break;
                    case "ELIMINAR":
                        q = $"SELECT * FROM empleados WHERE id_empleado = {id}";
                        if (!VerifyExistingValue(conn, q))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El dato no existe en la base de datos!");
                            return;
                        }
                        q = $"DELETE FROM empleados WHERE id_empleado = {id}";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato eliminado con exito!");
                        UpdateList();
                        Clean();
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

        private void Clean()
        {
            ID.Clear();
            Nombre.Clear();
            ApPaterno.Clear();
            ApMaterno.Clear();
            Clave.Clear();
            NumOficialia.SelectedValue = null;
            NumOficialia.SelectedItem = null;
            _clear = true;
            ID.IsEnabled = true;
            wasTriggered = false;
        }

        private bool AreAllTextBoxesEmptyExceptOne(TextBox exceptTextBox, params TextBox[] textBoxes)
        {
            return textBoxes.Except(new[] { exceptTextBox }).All(tb => string.IsNullOrEmpty(tb.Text));
        }

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox curr = (TextBox)sender;
            string txt = curr.Text;
            Debug.WriteLine(txt);
            if (string.IsNullOrEmpty(txt) && AreAllTextBoxesEmptyExceptOne(curr, TextBoxes) && _clear)
                UpdateList();
            else if (txt.Length > 0 && AreAllTextBoxesEmptyExceptOne(curr, TextBoxes) && _clear)
            {
                EnableButtons();
                EmpleadosList.ItemsSource = await GetEmpleadosAsync(txt, curr.Name);
            }
            if (EmpleadosList.Items.Count == 1)
            {
                EmpleadosList.SelectedIndex = 0;
                wasTriggered = true;
            }
        }

        private async Task<List<Empleados>> GetEmpleadosAsync(string value, string dataType)
        {
            List<Empleados> list = new List<Empleados>();
            Debug.WriteLine(Convert.ToInt32(value.ToUpper()) + " " +  dataType);
            value = value.ToUpper();
            foreach (Empleados obj in listData)
            {
                if (dataType == "ID" && obj.Id == Convert.ToInt32(value))
                    list.Add(obj);
                if (dataType == "Nombre" && obj.Nombre.ToUpper().Contains(value))
                    list.Add(obj);
                if(dataType == "ApPaterno" && obj.ApPaterno.ToUpper().Contains(value))
                    list.Add(obj);
                if (dataType == "ApMaterno" && obj.ApMaterno.ToUpper().Contains(value))
                    list.Add(obj);
            }
            return list;
        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                EnableButtons();
                Empleados sel = (Empleados)item;
                _clear = false;
                ID.Text = sel.Id.ToString();
                NumOficialia.SelectedItem = ElementosRegistros.FirstOrDefault(e => e._no_oficialia == sel.No_oficialia);
                Nombre.Text = sel.Nombre;
                ApPaterno.Text = sel.ApPaterno;
                ApMaterno.Text = sel.ApMaterno;
                Clave.Password = sel.Clave;
                if (!wasTriggered)
                    ID.IsEnabled = false;
            }
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

        private void ShowPass(object sender, RoutedEventArgs e)
        {
            string context = (sender as Button).Content.ToString().ToUpper();
            if (context == "VER CLAVE")
            {
                ButtonPass.Content = "Ocultar";
                Clave.Visibility = Visibility.Collapsed;
                ClaveShow.Visibility = Visibility.Visible;
                ClaveShow.Text = Clave.Password;
            }
            else
            {
                ButtonPass.Content = "Ver clave";
                Clave.Visibility = Visibility.Visible;
                ClaveShow.Visibility = Visibility.Collapsed;
                Clave.Password = ClaveShow.Text;
            }
        }

    }
}
