using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
    /// Interaction logic for Entidad.xaml
    /// </summary>
    public partial class EntidadView : UserControl
    {
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;
        List<Entidad> listData;
        public EntidadView()
        {
            InitializeComponent();
            CountryCB.ItemsSource = GetPaises();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            UpdateList();
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
                new ShoInfoMsg(ShoInfoMsg.ERROR, "Por favor asegurese que el campo Pais y ID tiene un valor! ");
                return;
            }
            if (!holder.CheckForValidText(id) || !holder.CheckForValidText(name) || !holder.CheckForValidText(country)) {
                new ShoInfoMsg(ShoInfoMsg.ERROR, "Datos ilegales detectados, intente de nuevo");
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
                            if(VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "ERROR!, valor repetido en la BD, intente de nuevo");
                                return;
                            }
                            insertQ = $"INSERT INTO entidad VALUES ('{id}','{country}','{name}')";
                            if(ExecQuery(conn, insertQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato insertado exitosamente!");
                                UpdateList();
                            }
                            break;
                        case "MODIFICAR":
                            searchQ = $"SELECT * FROM entidad WHERE id_entidad = '{id}'";
                            if(!VerifyExistingValue(conn, searchQ))
                            {
                                new ShoInfoMsg(ShoInfoMsg.ERROR, "Este dato no existe en la BD!");
                                return;
                            }
                            q = $"UPDATE entidad SET nombre = '{name}', id_pais = '{country}' WHERE id_entidad = '{id}'";
                            if(ExecQuery(conn, q))
                            {
                                new ShoInfoMsg(ShoInfoMsg.SUCCESS, "Dato actualizado con exito!");
                                UpdateList();   
                            }

                            break;
                    }
                }
            }
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
    }
}
