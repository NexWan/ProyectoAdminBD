using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Interaction logic for PaisView.xaml
    /// </summary>
    public partial class PaisView : UserControl
    {
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true, wasTriggered = false;
        List<Pais> listData;
        public PaisView()
        {
            InitializeComponent();
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            holder = DataHolder.Instance;
            UpdateList();
        }

        private void UpdateList()
        {
            List<Pais> data = new QueryExecutor().ExecuteQuery(
                holder.currentQuery,
                row => new Pais
                {
                    _id = row["id_pais"].ToString(),
                    _nombre = row["nombre"].ToString(),
                    _nacionalidad = row["nacionalidad"].ToString()
                }
                );
            myListView.ItemsSource = data;
            listData = data;
        }
        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            string id = IdBox.Text;
            string nac = NacBox.Text;
            string nombre = NombreBox.Text;
            string q;
            if (!holder.CheckForValidText(id) || !holder.CheckForValidText(nac) || !holder.CheckForValidText(nombre))
            {
                new ShoInfoMsg(ShoInfoMsg.ERROR, "Caracteres ilegales detectados!, intente denuevo");
                return;
            }
            Button btn = sender as Button;
            if (btn != null)
            {
                string context = btn.Content.ToString().ToUpper();
                Debug.WriteLine(context);
                switch (context)
                {
                    case "GUARDAR":
                        string searchQ = $"SELECT * FROM pais WHERE id_pais = '{id}'";
                        string insertQ = $"INSERT INTO pais VALUES ('{id}','{nombre}','{nac}')";
                        using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                        {
                            Debug.WriteLine(id.ToUpper()[0]);
                            conn.Open();
                            if (VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato ya existe en la BD, intente de nuevo!");
                                break;
                            }
                            if (ExecQuery(conn, insertQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado con exito!");
                                UpdateList();
                            }
                        }
                        break;
                    case "MODIFICAR":    
                        q = $"UPDATE PAIS SET nacionalidad = '{nac}', nombre = '{nombre}' WHERE id_pais = '{id}'";
                        if(NombreBox.Text == string.Empty || NacBox.Text == string.Empty)
                        {
                            bool conf = new ShoInfoMsg().ShowConfirmDialog("Uno de los valores esta vacio, esto reescribira el contenido de este a nada, desea continuar?");
                            if(!conf) { return; }
                        }
                        using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                        {
                            conn.Open();
                            if (ExecQuery(conn, q))
                            {
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                                UpdateList();
                                goto LIMPIAR;
                            }
                            else
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "No se encontro un valor con esa ID, intente de nuevo");
                            }
                        }
                        break;
                    case "ELIMINAR":
                        q = $"DELETE FROM pais WHERE id_pais = '{id}'";
                        string[] validation = new string[] { "abuelos", "padre", "madre", "entidad" };
                        if (id == string.Empty)
                        {
                            new ShoInfoMsg(ShoInfoMsg.WARNING, "No se recomienda eliminar valores sin un ID especifico, intente de nuevo.");
                            break;
                        }
                        using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                        {
                            conn.Open();
                            for(int i = 0; i < validation.Length; i++)
                            {
                                if(VerifyExistingValue(conn, $"SELECT * FROM {validation[i]} WHERE id_pais = '{id}'")){
                                    Debug.WriteLine($"SELECT * FROM {validation[i]} WHERE id_pais = '{id}'");
                                    new ShoInfoMsg(ShoInfoMsg.ERROR, "El dato a eliminar tiene hijos en la BD!");
                                    return;
                                }
                            }
                            if (ExecQuery(conn, q))
                            {
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato eliminado con exito!");
                                UpdateList();
                                goto LIMPIAR;
                            }
                        }
                        break;
                    case "LIMPIAR":
                        goto LIMPIAR;
                }
            }
        LIMPIAR:
            IdBox.Text = string.Empty;
            NacBox.Text = string.Empty;
            NombreBox.Text = string.Empty;
            IdBox.IsEnabled = true;
            DisableButtons();
            UpdateList();
            _clear = true;
            wasTriggered = false;
        }

        private bool VerifyExistingValue(SqlConnection conn, string query)
        {
            if (reader != null) reader.Close();
            cmd = conn.CreateCommand();
            cmd.CommandText = query;
            if (reader != null) reader.Close();
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
            TextBox currTxt = sender as TextBox;
            if (currTxt != null )
            {
                string txt = currTxt.Text;
                if (txt == string.Empty)
                    UpdateList();
                else if(txt !=  string.Empty && currTxt == IdBox && _clear)
                {
                    EnableButtons();
                    myListView.ItemsSource = await ExecQueryAsync(txt, "ID");
                }
                else if(txt != string.Empty && currTxt != IdBox && _clear) { 
                    EnableButtons();
                    string passed = (currTxt == NombreBox) ? "NOMBRE" : (currTxt == NacBox) ? "NAC" : "Default";
                    Debug.WriteLine(passed);
                    myListView.ItemsSource = await ExecQueryAsync(txt,passed);
                }
            }
            if (myListView.Items.Count == 1)
            {
                myListView.SelectedIndex = 0;
                wasTriggered = true;
            }
        }

        private async Task<List<Pais>> ExecQueryAsync(string id, string type)
        {
            List<Pais> presentados = new List<Pais>();
            foreach (Pais obj in listData)
            {
                if (obj._id.ToUpper().Contains(id.ToUpper()) && type == "ID")
                    presentados.Add(obj);
                if (obj._nombre.ToUpper().Contains(id.ToUpper()) && type == "NOMBRE")
                    presentados.Add(obj);
                if(obj._nacionalidad.ToUpper().Contains(id.ToUpper()) && type == "NAC")
                    presentados.Add(obj);
            }
            return presentados;
        }

        private void SelectManager(object sender, SelectionChangedEventArgs e) 
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                _clear = false;
                IdBox.Text = ((Pais)item)._id;
                NombreBox.Text = ((Pais)item)._nombre;
                NacBox.Text = ((Pais)item)._nacionalidad;
                EnableButtons();
                if (!wasTriggered)
                    IdBox.IsEnabled = false;
            }
        }
    }
}
