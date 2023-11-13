using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.Theme;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ProyectoAdminBD.MVVM.Model;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ProyectoAdminBD.MVVM.View
{
    /// <summary>
    /// Lógica de interacción para PresentadoView.xaml
    /// </summary>
    public partial class PresentadoView : UserControl
    {
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;
        List<Presentado> listData;
        public PresentadoView()
        {
            InitializeComponent();
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            holder = DataHolder.Instance;
            UpdateList();
        }

        private void UpdateList()
        {
            List<Presentado> data = new QueryExecutor().ExecuteQuery(
                holder.currentQuery,
                row => new Presentado
                {
                    _id = row["id_PRESENTADO"].ToString(),
                    _descripcion = row["Descripcion"].ToString()
                }
                );
            myListView.ItemsSource = data;
            listData = data;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {
            string id = IdBox.Text;
            string desc = DescBox.Text;
            string q;
            if (!holder.CheckForValidText(id) || !holder.CheckForValidText(desc))
            {
                new ShoInfoMsg("ERROR", "Caracteres ilegales detectados!, intente denuevo");
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
                        string searchQ = $"SELECT * FROM presentado WHERE id_presentado = '{id}'";
                        string insertQ = $"INSERT INTO presentado VALUES ('{id}','{desc}')";
                        using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                        {
                            Debug.WriteLine(id.ToUpper()[0]);
                            conn.Open();
                            if (VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg("ERROR", "Este dato ya existe en la BD, intente de nuevo!");
                                break;
                            }
                            if (id.ToUpper()[0] != 'V' && id.ToUpper()[0] != 'M')
                            {
                                new ShoInfoMsg("ERROR", "Solo se pueden insertar ID con el valor de V o M, intente de nuevo");
                                break;
                            }
                            if (ExecQuery(conn, insertQ))
                            {
                                new ShoInfoMsg("SUCCESS", "Dato insertado con exito!");
                                UpdateList();
                            }
                        }
                        break;
                    case "MODIFICAR":
                        q = $"UPDATE presentado SET descripcion = '{desc}' WHERE id_presentado = '{id}'";
                        using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                        {
                            conn.Open();
                            if (ExecQuery(conn, q))
                            {
                                new ShoInfoMsg("SUCCESS", "Dato actualizado con exito!");
                                UpdateList();
                                goto LIMPIAR;
                            }
                            else
                            {
                                new ShoInfoMsg("ERROR", "No se encontro un valor con esa ID, intente de nuevo");
                            }
                        }
                        break;
                    case "ELIMINAR":
                        q = $"DELETE FROM presentado WHERE id_presentado = '{id}'";
                        if (id == string.Empty)
                        {
                            new ShoInfoMsg("WARNING", "No se recomienda eliminar valores sin un ID especifico, intente de nuevo.");
                            break;
                        }
                        using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
                        {
                            conn.Open();
                            if (VerifyExistingValue(conn, $"SELECT * FROM persona_registrada WHERE idpresentado = '{id}'"))
                            {
                                new ShoInfoMsg("ERROR", "Este dato es hijo de una tabla!, no se puede eliminar");
                                break;
                            }
                            if (ExecQuery(conn, q))
                            {
                                new ShoInfoMsg("SUCCESS", "Dato eliminado con exito!");
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
            DescBox.Text = string.Empty;
            IdBox.IsEnabled = true;
            DisableButtons();
            UpdateList();
            _clear = true;

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

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {
            TextBox IdTxtBox = sender as TextBox;
            if (IdTxtBox != null)
            {
                string txt = IdTxtBox.Text;
                if (txt == string.Empty)
                    UpdateList();
                else if(txt != string.Empty && IdTxtBox.Name == "IdBox" && _clear)
                {
                    EnableButtons();
                    myListView.ItemsSource = await ExecQueryAsync(txt, "ID");
                    if(myListView.Items.Count == 1)
                        myListView.SelectedIndex = 0;
                }
            }
        }

        private async Task<List<Presentado>> ExecQueryAsync(string id, string type)
        {
            List<Presentado> presentados = new List<Presentado>();
            foreach(Presentado obj in listData)
            {
                if(obj._id.ToUpper().Contains(id.ToUpper()) && type == "ID")
                    presentados.Add(obj);
                if (obj._descripcion.ToUpper().Contains(id.ToUpper()) && type == "DESC")
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
                IdBox.Text = ((Presentado)item)._id;
                IdBox.IsEnabled = false;
                DescBox.Text = ((Presentado)item)._descripcion;
                EnableButtons();
            }
        }
    }
}
