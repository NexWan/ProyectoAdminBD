using System;
using System.Windows;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.Theme;
using System.Data;

namespace ProyectoAdminBD.MVVM.Model
{
    class VMModel
    {
        private IConfiguration _configuration;
        public VMModel(String query) { 
            if(!DataHolder.Instance.CheckForInternetConnection()) {
                MessageBox.Show("No hay conexion a internet!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            var appconfig = new LoadConfig();
            _configuration = appconfig.Configuration;
            DataTable dt = new DataTable();
            using(SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                SqlDataAdapter adapter= new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(query, conn);
                adapter.Fill(dt);
            }
            DV = dt.DefaultView;
        }

        public DataView DV { get; set; }
    }
}
