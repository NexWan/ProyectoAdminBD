using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
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

        
            public SqlConnection GetConnection()
            {
            string keyFilePath = "evident-ethos-400620-d4ba39dfc502.json";
            GoogleCredential credential = GoogleCredential.FromFile(keyFilePath);
            StorageClient storageClient = StorageClient.Create(credential);
            Debug.WriteLine("Authenticated successfully.");

            try
            {
                sqlCon = new SqlConnection(_configuration.GetConnectionString("AdminConnection"));
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrio un error con la conexion!, codigo de error: \n{e}");
            }
            return sqlCon;
        }

        }
}
