using System;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace ProyectoAdminBD.Connection
{
    class SqlConn
    {

        private readonly IConfiguration _configuration;
        private SqlConnection? sqlCon;

        public SqlConn(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public SqlConnection? GetConnection()
        {
            try
            {
                sqlCon = new SqlConnection(_configuration.GetConnectionString("AdminConnection"));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrio un error con la conexion!, codigo de error: \n{e}");
            }
            if( sqlCon != null )
            {
                return sqlCon;
            }
            else
            {
                return null;
            }
        }
    }
}
