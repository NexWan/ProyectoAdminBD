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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para PaternalesView.xaml
    /// </summary>
    public partial class PaternalesView : UserControl
    {
        TextBox[] textBoxesPadres = { };
        TextBox[] textBoxesAbuelos = { };
        bool wasTriggered = false;
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;
        List<Padres> listDataPadres;
        List<Abuelos> listDataAbuelos;
        List<Pais> dataPais;
        List<string> parentOptions = new List<string>() { "Padre","Madre" };
        List<Genero> listGenero;
        public PaternalesView()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            CountryCB.ItemsSource = GetPaises();
            CountryCB2.ItemsSource = CountryCB.ItemsSource;
            GeneroBox.ItemsSource = GetGeneros();
            DisableButtons("Padres");
            DisableButtons("Abuelos");
            UpdateListPadres();
            UpdateListAbuelos();
            textBoxesPadres = new TextBox[] {CURP, NOMBRES_PADRES,  Apellido_M_Padres, Apellido_P_Padres, Edad_Padres};
            textBoxesAbuelos = new TextBox[] {ID_ABUELO,NOMBRES_ABUELO, APELLIDO_M_ABUELO, APELLIDO_P_ABUELO };
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
            dataPais = data;
            return data;
        }

        private void UpdateListPadres()
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
            listDataPadres = combinedData;
        }

        private void UpdateListAbuelos()
        {
            List<Abuelos> data = new QueryExecutor().ExecuteQuery(
                "SELECT * FROM Abuelos",
                row => new Abuelos
                {
                    _id = row["id_abuelo"].ToString(),
                    _pais = row["id_pais"].ToString(),
                    _genero = row["id_genero"].ToString(),
                    _nombres = row["nomAbuelo"].ToString(),
                    _ap_paterno = row["apPaterno"].ToString(),
                    _ap_materno = row["apMaterno"].ToString()
                }
                );
            TablaAbuelos.ItemsSource = data;
            listDataAbuelos = data;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            string btName = button.Name;
            string context = button.Content.ToString().ToUpper();
            Debug.WriteLine(btName);
            if (btName[btName.Length - 1] == 'P')
                PadresTasks(context);
            else
                AbuelosTasks(context);
        }

        private void PadresTasks(string context)
        {
            string curp = CURP.Text;
            string? country = CountryCB?.SelectedValue?.ToString();
            string names = NOMBRES_PADRES.Text;
            string lastNameF = Apellido_P_Padres.Text;
            string lastNameM = Apellido_M_Padres.Text;
            var age = Edad_Padres.Text;
            string? kinship = (Parentesco_Padres?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string insertQ, searchQ, q;
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
                        UpdateListPadres();
                        Clean("Padres");
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
                        q = $"UPDATE {kinship} SET nombres = '{names}', id_pais = '{country}',ap_materno = '{lastNameM}',ap_paterno = '{lastNameF}', edad = '{age}' WHERE curp = '{curp}'";
                        if(ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                        UpdateListPadres();
                        Clean("Padres");
                        break;
                    case "ELIMINAR":
                        if (string.IsNullOrEmpty(curp) || string.IsNullOrEmpty(kinship))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "CURP Y parentezco no puede estar vacio!");
                            return;
                        }
                        searchQ = $"SELECT * FROM {kinship} WHERE curp = '{curp}'";
                        if(!VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El dato no existe en la BD!");
                            return;
                        }
                        searchQ = $"SELECT * FROM persona_registrada WHERE {kinship}_curp = '{curp}'";
                        Debug.WriteLine(searchQ);
                        if (VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El dato tiene hijos en la BD!");
                            return;
                        }
                        q = $"DELETE FROM {kinship} WHERE curp = '{curp}'";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato eliminado con exito!");
                        UpdateListPadres();
                        Clean("Padres");
                        break;
                    case "LIMPIAR":
                        Clean("Padres");
                        break;
                }
            }   
        }

        private int GetAmountAbuelos()
        {
            int amount = 0;
            string x = "0";
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                string q = "SELECT id_abuelo FROM abuelos";
                conn.Open();
                cmd = conn.CreateCommand();
                cmd.CommandText = q;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                    x = reader.GetString(0);
            }
            if(x != "0")
                amount = Convert.ToInt32(x);
            return amount;
        }

        private void AbuelosTasks(string context)
        {
            var id = ID_ABUELO.Text;
            string? country = CountryCB2?.SelectedValue?.ToString();
            string names = NOMBRES_ABUELO.Text;
            string? gender = GeneroBox?.SelectedValue?.ToString();
            string lastNameF = APELLIDO_P_ABUELO.Text;
            string lastNameM = APELLIDO_M_ABUELO.Text;
            string insertQ, searchQ, q;
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                Debug.WriteLine(country + " " +  names + " " + gender + " " +  lastNameF + " " + lastNameM);
                conn.Open();
                switch (context)
                {
                    case "GUARDAR":
                        if (string.IsNullOrEmpty(names) || country == null || gender == null || string.IsNullOrEmpty(lastNameF) || string.IsNullOrEmpty(lastNameM))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Por favor llene todos los datos para insertar!");
                            return;
                        }
                        if (!(holder.CheckForValidText(names)  && holder.CheckForValidText(lastNameM) && holder.CheckForValidText(lastNameF)))
                        {
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "Caracteres ilegales detectados! intente de nuevo");
                            return;
                        }
                        int insertId = GetAmountAbuelos() + 1;
                        insertQ = $"INSERT INTO Abuelos VALUES('{insertId}','{country}','{gender}','{names}','{lastNameF}','{lastNameM}')";
                        if (ExecQuery(conn, insertQ))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                        UpdateListAbuelos();
                        Clean("Abuelos");
                        break;
                    case "MODIFICAR":
                        if (country == null || gender == null)
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Pais o genero no pueden estar vacios, intente de nuevo!");
                            return;
                        }
                        searchQ = $"SELECT * FROM Abuelos WHERE id_abuelo = '{id}'";
                        if (!VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato no existe en la bd!");
                            return;
                        }
                        if (string.IsNullOrEmpty(names) || string.IsNullOrEmpty(lastNameM) || string.IsNullOrEmpty(lastNameF))
                        {
                            bool conf = new ShoInfoMsg().ShowConfirmDialog("Algunos campos se encuentra vacios, estos se reescribiran si no los llena, desea continuar?");
                            if (!conf) return;
                        }
                        q = $"UPDATE Abuelos SET nomAbuelo = '{names}', id_pais = '{country}',apPaterno = '{lastNameM}',apMaterno = '{lastNameF}',id_genero = '{gender}'  WHERE id_abuelo = '{id}'";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                        UpdateListAbuelos();
                        Clean("Abuelos");
                        break;
                    case "ELIMINAR":
                        if (string.IsNullOrEmpty(id))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "Seleccione un ID valido!");
                            return;
                        }
                        searchQ = $"SELECT * FROM Abuelos WHERE id_abuelo = '{id}'";
                        if (!VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El dato no existe en la BD!");
                            return;
                        }
                        searchQ = $"SELECT * FROM personaAbuelos WHERE id_abuelo = '{id}'";
                        Debug.WriteLine(searchQ);
                        if (VerifyExistingValue(conn, searchQ))
                        {
                            new ShoInfoMsg(ShoInfoMsg.ERROR, "El dato tiene hijos en la BD!");
                            return;
                        }
                        q = $"DELETE FROM abuelos WHERE id_abuelo = '{id}'";
                        if (ExecQuery(conn, q))
                            new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato eliminado con exito!");
                        UpdateListAbuelos();
                        Clean("Abuelos");
                        break;
                    case "LIMPIAR":
                        Clean("Abuelos");
                        break;
                }
            }
        }

        private void Clean(string task)
        {
            if(task == "Padres")
            {
                CURP.Text = string.Empty;
                CountryCB.SelectedItem = null;
                CountryCB.SelectedValue = null;
                NOMBRES_PADRES.Text = string.Empty;
                Apellido_M_Padres.Text = string.Empty;
                Apellido_P_Padres.Text = string.Empty;
                Edad_Padres.Text = string.Empty;
                Parentesco_Padres.SelectedValue = null;
                DisableButtons("Padres");
                UpdateListPadres();
            }
            else if(task == "Abuelos")
            {
                ID_ABUELO.Text = string.Empty;
                NOMBRES_ABUELO.Text= string.Empty;
                APELLIDO_M_ABUELO.Text = string.Empty;
                APELLIDO_P_ABUELO.Text = string.Empty;
                CountryCB2.SelectedItem = null;
                CountryCB2.SelectedValue = null;
                GeneroBox.SelectedItem = null;
                GeneroBox.SelectedValue = null;
                DisableButtons("Abuelos");
                UpdateListAbuelos();
            }
            wasTriggered = false;
            _clear = true;
        }


        private bool ExecQuery(SqlConnection conn, string query)
        {
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
            listGenero = data;
            return data;
        }

        private void ValidateNumeric(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TablaPadres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                EnableButtons("Padres");
                Padres padres = (Padres)item;
                _clear = false;
                CURP.Text = padres._curp;
                CountryCB.SelectedItem = dataPais.FirstOrDefault(p => p._id == padres._pais);
                Parentesco_Padres.SelectedValue = padres._parentezco;
                NOMBRES_PADRES.Text = padres._nombres;
                Apellido_M_Padres.Text = padres._ap_materno;
                Apellido_P_Padres.Text = padres._ap_paterno;
                Edad_Padres.Text = Convert.ToString(padres._edad);
            }
        }

        private void CheckBoxSelection(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            object selectedItemn = comboBox.SelectedValue;
            Debug.WriteLine(selectedItemn);
        }

        private void DisableButtons(string type)
        {
            if(type == "Padres")
            {
                SaveP.IsEnabled = false;
                UpdateP.IsEnabled = false;
                DeleteP.IsEnabled = false;
                ClearP.IsEnabled = false;
            }
            else{
                SaveA.IsEnabled = false;
                UpdateA.IsEnabled = false;
                DeleteA.IsEnabled = false;
                ClearA.IsEnabled = false;
            }
        }

        public void EnableButtons(string type)
        {
            if (type == "Padres")
            {
                SaveP.IsEnabled = true;
                UpdateP.IsEnabled = true;
                DeleteP.IsEnabled = true;
                ClearP.IsEnabled = true;
            }
            else
            {
                SaveA.IsEnabled = true;
                UpdateA.IsEnabled = true;
                DeleteA.IsEnabled = true;
                ClearA.IsEnabled = true;
            }
        }

        private async void SearchByTextPadres(object sender, TextChangedEventArgs e)
        {
            TextBox curr = (TextBox)sender;
            string dataType;
            string txt = curr.Text;
            if (txt == string.Empty && AreAllTextBoxesEmptyExceptOne(curr, textBoxesPadres))
            {
                UpdateListPadres();
                return;
            }
            else if (txt != string.Empty && _clear && AreAllTextBoxesEmptyExceptOne(curr, textBoxesPadres))
            {
                GetContextData(curr.Name, out dataType);
                EnableButtons("Padres");
                TablaPadres.ItemsSource = await GetPadresAsync(txt, dataType);
            }
            if(TablaPadres.Items.Count == 1)
                TablaPadres.SelectedIndex = 0;
        }

        private bool AreAllTextBoxesEmptyExceptOne(TextBox exceptTextBox, params TextBox[] textBoxes)
        {
            return textBoxes.Except(new[] { exceptTextBox }).All(tb => string.IsNullOrEmpty(tb.Text));
        }

        private async Task<List<Padres>> GetPadresAsync(string value, string dataType)
        {
            List<Padres> list = new List<Padres>();
            value = value.ToUpper();
            foreach(Padres obj in listDataPadres)
            {
                if (obj._curp.ToUpper().Contains(value) && dataType == "CURP")
                    list.Add(obj);
                if (obj._pais.ToUpper().Contains(value) && dataType == "id_pais")
                    list.Add(obj);
                if (obj._nombres.ToUpper().Contains(value) && dataType == "Nombres")
                    list.Add(obj);
                if (obj._ap_paterno.ToUpper().Contains(value) && dataType == "ap_paterno")
                    list.Add(obj);
                if (obj._ap_materno.ToUpper().Contains(value) && dataType == "ap_materno")
                    list.Add(obj);
                if (IsNumeric(value) && obj._edad == Convert.ToInt32(value) && dataType == "edad")
                    list.Add(obj);
            }
            return list;
        }

        private async void SearchByTextAbuelos(object sender, TextChangedEventArgs e)
        {
            TextBox curr = (TextBox)sender;
            string dataType;
            string txt = curr.Text;
            if (txt == string.Empty && AreAllTextBoxesEmptyExceptOne(curr, textBoxesAbuelos))
            {
                UpdateListAbuelos();
                return;
            }
            else if (txt != string.Empty && _clear && AreAllTextBoxesEmptyExceptOne(curr, textBoxesAbuelos))
            {
                GetContextData(curr.Name, out dataType);
                EnableButtons("Abuelos");
                TablaAbuelos.ItemsSource = await GetAbuelosAsync(txt, dataType);
            }
            if (TablaAbuelos.Items.Count == 1)
                TablaAbuelos.SelectedIndex = 0;
        }

        private async Task<List<Abuelos>> GetAbuelosAsync(string value, string dataType)
        {
            List<Abuelos> list = new List<Abuelos>();
            value = value.ToUpper();
            foreach (Abuelos obj in listDataAbuelos)
            {
                if (obj._id.ToUpper().Contains(value) && dataType == "ID")
                    list.Add(obj);
                if (obj._pais.ToUpper().Contains(value) && dataType == "id_pais")
                    list.Add(obj);
                if (obj._nombres.ToUpper().Contains(value) && dataType == "Nombres")
                    list.Add(obj);
                if (obj._ap_paterno.ToUpper().Contains(value) && dataType == "ap_paterno")
                    list.Add(obj);
                if (obj._ap_materno.ToUpper().Contains(value) && dataType == "ap_materno")
                    list.Add(obj);
                if (obj._genero.ToUpper().Contains(value) && dataType == "GENERO")
                    list.Add(obj);
            }
            return list;
        }

        public static bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

        private void GetContextData(in string name, out string dataType)
        {
            switch(name.ToUpper())
            {
                case "CURP":
                    dataType = "CURP";
                    break;
                case "ID_ABUELO":
                    dataType = "ID";
                    break;
                case "NOMBRES_ABUELO":
                case "NOMBRES_PADRES":
                    dataType = "Nombres";
                    break;
                case "COUNTRYCB2":
                case "COUNTRYCB":
                    dataType = "id_pais";
                    break;
                case "APELLIDO_P_ABUELO":
                case "APELLIDO_P_PADRES":
                    dataType = "ap_paterno";
                    break;
                case "APELLIDO_M_ABUELO":
                case "APELLIDO_M_PADRES":
                    dataType = "ap_materno";
                    break;
                case "EDAD_PADRES":
                    dataType = "edad";
                    break;
                case "GENEROBOX":
                    dataType = "id_genero";
                    break;
                default:
                    dataType = string.Empty;
                    break;
            }
        }

        private void TablaAbuelos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                EnableButtons("Abuelos");
                Abuelos abuelos = (Abuelos)item;
                _clear = false;
                ID_ABUELO.Text = abuelos._id;
                CountryCB2.SelectedItem = dataPais.FirstOrDefault(p => p._id == abuelos._pais);
                GeneroBox.SelectedItem = listGenero.FirstOrDefault(g => g.Id == abuelos._genero);
                NOMBRES_ABUELO.Text = abuelos._nombres;
                APELLIDO_M_ABUELO.Text = abuelos._ap_materno;
                APELLIDO_P_ABUELO.Text = abuelos._ap_paterno;
            }
        }
    }
}
