using System;
using System.Windows;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProyectoAdminBD.Connection;
using ProyectoAdminBD.Theme;
using System.Data;
using System.Collections.Generic;

namespace ProyectoAdminBD.MVVM.Model
{
    class QueryExecutor
    {
        private IConfiguration _configuration;

        public QueryExecutor()
        {
        }
        public List<T> ExecuteQuery<T>(String query, Func<SqlDataReader, T> mapRowFunc)
        {
            if (!DataHolder.Instance.CheckForInternetConnection())
            {
                MessageBox.Show("No hay conexion a internet!", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            List<T> results = new List<T>();
            var appconfig = new LoadConfig();
            _configuration = appconfig.Configuration;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConn(_configuration).GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T mappedData = mapRowFunc(reader);
                            results.Add(mappedData);
                        }
                    }
                }
            }
            return results;
        }
    }
}
