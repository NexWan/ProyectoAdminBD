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
    /// Interaction logic for Municipio.xaml
    /// </summary>
    public partial class MunicipioView : UserControl
    {
        private IConfiguration _configuration;
        DataHolder holder;
        SqlCommand cmd;
        SqlDataReader reader;
        bool _clear = true;
        List<Municipio> listData;
        List<Pais> paises;
        public MunicipioView()
        {
            InitializeComponent();
            holder = DataHolder.Instance;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
            UpdateList();
        }

        private void UpdateList()
        {
            List<Municipio> data = new QueryExecutor().ExecuteQuery(
               holder.currentQuery,
               row => new Municipio
               {
                   _id = row["id_municipio"].ToString(),
                   _entidad = row["id_entidad"].ToString(),
                   _name = row["nombre"].ToString()
               }
               );
            MunicipioListView.ItemsSource = data;
            listData = data;
        }

        private void ExecOperation(object sender, RoutedEventArgs e)
        {

        }

        private void CountryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void SearchByTextBox(object sender, TextChangedEventArgs e)
        {

        }

        private void SelectManager(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}
