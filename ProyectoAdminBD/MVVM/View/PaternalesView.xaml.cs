using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para PaternalesView.xaml
    /// </summary>
    public partial class PaternalesView : UserControl
    {
        bool wasTriggered = false;
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;
        public PaternalesView()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            CountryCB.ItemsSource = GetPaises();
            CountryCB2.ItemsSource = CountryCB.ItemsSource;
            GeneroBox.ItemsSource = GetGeneros();
            UpdateList();
        }

        private List<Pais> GetPaises()
        {
            List<Pais> data = new QueryExecutor().ExecuteQuery(
                "SELECT id_pais, nombre, nacionalidad FROM pais",
                row => new Pais
                {
                    _id = row["id_pais"].ToString(),
                    _nombre = row["nombre"].ToString(),
                    _nacionalidad = row["nacionalidad"].ToString()
                }
                );
            return data;
        }

        private void UpdateList()
        {
            List<Padres> dataPadres = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM padre",
                row => new Padres
                {
                    _curp = row["curp"].ToString(),
                    _pais = row["id_pais"].ToString(),
                    _nombres = row["nombres"].ToString(),
                    _ap_paterno = row["ap_paterno"].ToString(),
                    _ap_materno = row["ap_materno"].ToString(),
                    _edad = Convert.ToInt32(row["edad"]),
                    _parentezco = "Padre"
                });

            List<Padres> dataMadres = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM madre",
                row => new Padres
                {
                    _curp = row["curp"].ToString(),
                    _pais = row["id_pais"].ToString(),
                    _nombres = row["nombres"].ToString(),
                    _ap_paterno = row["ap_paterno"].ToString(),
                    _ap_materno = row["ap_materno"].ToString(),
                    _edad = Convert.ToInt32(row["edad"]),
                    _parentezco = "Madre"
                });
            List<Padres> combinedData = dataPadres.Concat(dataMadres).ToList();
            TablaPadres.ItemsSource = combinedData;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            string btName = button.Name;
            string context = button.Content.ToString().ToUpper();
            Debug.WriteLine(btName);
            if (btName[btName.Length - 1] == 'P')
            {
                PadresTasks(context);
            }
            else
            {
                Debug.WriteLine("Operaciones abuelos");
            }
        }

        private void PadresTasks(string context)
        {
            string curp = CURP.Text;
            string? country = CountryCB?.SelectedValue?.ToString();
            string names = NOMBRES_PADRES.Text;
            string lastNameF = Apellido_P_Padres.Text;
            string lastNameM = Apellido_M_Padres.Text;
            var age = Edad_Padres.Text;
            string? kinship = (Parentesco_Padres.SelectedItem as ComboBoxItem)?.Content.ToString();
            string insertQ, searchQ, q;
            Debug.WriteLine(kinship,"", country);
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                conn.Open();
                switch (context)
                {
                    case "GUARDAR":
                        if (string.IsNullOrEmpty(curp) || country == null || string.IsNullOrEmpty(names)
                            || string.IsNullOrEmpty(lastNameF) || string.IsNullOrEmpty(lastNameM) || string.IsNullOrEmpty(age)
                            || string.IsNullOrEmpty(kinship))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Por favor llene todos los datos para insertar!");
                            return;
                        }
                        if (!(holder.CheckForValidText(curp) && holder.CheckForValidText(names) && holder.CheckForValidText(lastNameM) && holder.CheckForValidText(lastNameF)))
                        {
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "Caracteres ilegales detectados! intente de nuevo");
                            return;
                        }
                        searchQ = $"SELECT * FROM {kinship} WHERE curp = '{curp}'";
                        if (VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "Este valor ya existe en la BD!, intente de nuevo");
                            return;
                        }
                        insertQ = $"INSERT INTO {kinship} VALUES('{curp}','{country}','{names}','{lastNameF}','{lastNameM}',{age})";
                        if (ExecQuery(conn, insertQ))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                        UpdateList();
                        break;
                    case "MODIFICAR":
                        if(string.IsNullOrEmpty(curp) || country == null || string.IsNullOrEmpty(kinship)){
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Los valores CURP, Pais o Parentezco no pueden estar vacios, intente de nuevo!");
                            return;
                        }
                        searchQ = $"SELECT * FROM {kinship} WHERE curp = '{curp}'";
                        if(!VerifyExistingValue(conn, searchQ)) {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato no existe en la bd!");
                            return;
                        }
                        if (string.IsNullOrEmpty(names)  || string.IsNullOrEmpty(lastNameM) ||  string.IsNullOrEmpty(lastNameF) || string.IsNullOrEmpty(age)) {
                            bool conf = new ShoInfoMsg().ShowConfirmDialog("Algunos campos se encuentra vacios, estos se reescribiran si no los llena, desea continuar?");
                            if (!conf) return;
                        }

                        break;
                }
            }
        }

        private void AbuelosTasks(string context)
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

        private bool VerifyExistingValue(SqlConnection conn, string query)
        {
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


        private List<Genero> GetGeneros()
        {
            List<Genero> data = new QueryExecutor().ExecuteQuery(
                "SELECT id_genero, DESCRIPCION FROM genero",
                row => new Genero
                {
                    Id = row["Id_GENERO"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                }
                );
            return data;
        }

        private void CountryCB_Selected(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void CountryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            // You can access the selected item or selected value from the ComboBox
            object selectedItem = comboBox.SelectedItem;
            object selectedValue = comboBox.SelectedValue; // Assu

            Debug.WriteLine(selectedValue);
        }

        private void ValidateNumeric(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
