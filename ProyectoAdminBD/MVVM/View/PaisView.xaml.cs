using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.MVVM.Model;
using ProyectoAdminBD.Theme;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        bool _clear = true;
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
                ) ;
            myListView.ItemsSource = data;
            listData = data;
        }
        private void ExecOperation(object sender, RoutedEventArgs e)
        {}

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {

        }

        private void SelectManager(object sender, SelectionChangedEventArgs e) { }
    }
}
