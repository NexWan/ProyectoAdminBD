using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Interaction logic for Entidad.xaml
    /// </summary>
    public partial class EntidadView : UserControl
    {
        bool wasTriggered = false;
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;
        List<Entidad> listData;
        List<Pais> paises;
        public EntidadView()
        {
            InitializeComponent();
            CountryCB.ItemsSource = GetPaises();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            UpdateList();
            DisableButtons();
        }

        private void UpdateList()
        {
            List<Entidad> data = new QueryExecutor().ExecuteQuery(
                holder.currentQuery,
                row => new Entidad
                {
                    _id = row["id_entidad"].ToString(),
                    _pais = row["id_pais"].ToString(),
                    _nombre = row["nombre"].ToString()
                }
                );
            EntidadListView.ItemsSource = data;
            listData = data;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            string id = IdBox.Text;
            string name = NombreBox.Text;
            string? country = CountryCB?.SelectedValue?.ToString();
            if (country == null || id == string.Empty)
            {
                new ShoInfoMsg(ShoInfoMsg.WARNING, "Por favor asegurese que el campo Pais y ID tiene un valor! ");
                return;
            }
            if (!holder.CheckForValidText(id) || !holder.CheckForValidText(name) || !holder.CheckForValidText(country)) {
                new ShoInfoMsg(ShoInfoMsg.WARNING, "Datos ilegales detectados, intente de nuevo");
                return;
            }
            Button? btn = sender as Button;
            if( btn != null )
            {
                string? context = btn.Content.ToString();
                Debug.WriteLine(context);
                string searchQ, insertQ, q;
                using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                {
                    conn.Open();
                    switch (context.ToUpper())
                    {
                        case "GUARDAR":
                            if (country == null || id == string.Empty || name == string.Empty)
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "ERROR!, uno de los datos esta vacio!");
                                return;
                            }
                            searchQ = $"SELECT * FROM entidad WHERE id_entidad = '{id}'";
                            if (VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "ERROR!, valor repetido en la BD, intente de nuevo");
                                return;
                            }
                            insertQ = $"INSERT INTO entidad VALUES ('{id}','{country}','{name}')";
                            if (ExecQuery(conn, insertQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado exitosamente!");
                                UpdateList();
                            }
                            goto LIMPIAR;
                        case "MODIFICAR":
                            searchQ = $"SELECT * FROM entidad WHERE id_entidad = '{id}'";
                            if (!VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato no existe en la BD!");
                                return;
                            }
                            q = $"UPDATE entidad SET nombre = '{name}', id_pais = '{country}' WHERE id_entidad = '{id}'";
                            if (ExecQuery(conn, q))
                            {
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                                UpdateList();
                                goto LIMPIAR;
                            }
                            break;
                        case "ELIMINAR":
                            searchQ = $"SELECT * FROM entidad WHERE id_entidad = '{id}'";
                            if (!VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "ERROR!, dato no encontrado en la BD, intente de nuevo");
                                return;
                            }
                            searchQ = $"SELECT * FROM municipio WHERE id_entidad = '{id}'";
                            if (VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato tiene hijos!, no se puede eliminar");
                                return;
                            }
                            q = $"DELETE FROM entidad WHERE id_entidad = '{id}'";
                            if (ExecQuery(conn, q))
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato eliminado con exito!");
                            UpdateList();
                            goto LIMPIAR;
                        case "LIMPIAR":
                            goto LIMPIAR;
                    }
                }
            }
        LIMPIAR:
            IdBox.Text = string.Empty;
            CountryCB.SelectedItem = null;
            CountryCB.SelectedValue = null;
            NombreBox.Text = string.Empty;
            IdBox.IsEnabled = true;
            DisableButtons();
            UpdateList();
            wasTriggered = false;
            _clear = true;
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
            paises = data;
            return data;
        }

        private void CountryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            // You can access the selected item or selected value from the ComboBox
            object selectedItem = comboBox.SelectedItem;
            object selectedValue = comboBox.SelectedValue; // Assu

            Debug.WriteLine(selectedValue);
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

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox currTxt = (TextBox)sender;
            string txt = currTxt.Text;
            if (txt == string.Empty)
                UpdateList();
            else if(txt != string.Empty)
            {
                if(txt != string.Empty && currTxt == IdBox && _clear)
                {
                    EnableButtons();
                    EntidadListView.ItemsSource = await GetEntidadsAsync(txt, "ID");
                }
                else if (txt != string.Empty && currTxt != IdBox && _clear)
                {
                    EnableButtons();
                    EntidadListView.ItemsSource = await GetEntidadsAsync(txt, "NOMBRE");
                }
            }
            if (EntidadListView.Items.Count == 1)
            {
                EntidadListView.SelectedIndex = 0;
                wasTriggered = true;
            }
        }

        private async Task<List<Entidad>> GetEntidadsAsync(string id, string type)
        {
            List<Entidad> list = new List<Entidad>();
            foreach(Entidad obj in listData)
            {
                if(obj._id.ToUpper().Contains(id.ToUpper()) && type == "ID")
                    list.Add(obj);
                if (obj._nombre.ToUpper().Contains(id.ToUpper()) && type == "NOMBRE")
                    list.Add(obj);
            }
            return list;
        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                _clear = false;
                IdBox.Text = ((Entidad)item)._id;
                NombreBox.Text = ((Entidad)item)._nombre;
                CountryCB.SelectedItem = paises.FirstOrDefault(p => p._id == ((Entidad)item)._pais);
                EnableButtons();
            }
            if (!wasTriggered)
            {
                IdBox.IsEnabled = false;
            }
            Debug.WriteLine(wasTriggered);
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
    }
}
