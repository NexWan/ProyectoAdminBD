using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace ProyectoAdminBD.Connection
{
        class SqlConn
        {
        private SqlConnection sqlCon;

            public SqlConn(String user, String pwd, String db)
            {
            String conn = $"Server=localhost\\SQLEXPRESS03;Database={db};Trusted_Connection=True;";
            sqlCon = new SqlConnection(conn);
            }

        public SqlConn(String db)
        {
            String conn = $"Server=localhost\\SQLEXPRESS03;Database={db};Trusted_Connection=True;";
            sqlCon = new SqlConnection(conn);
        }
        
            public SqlConnection GetConnection()
            {
                return sqlCon;
            }

        }
}
