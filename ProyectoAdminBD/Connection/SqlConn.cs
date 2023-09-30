***REMOVED***
using System.Collections.Generic;
***REMOVED***
using System.Linq;
using System.Text;
using System.Threading.Tasks;
***REMOVED***
using Microsoft.SqlServer.Server;

***REMOVED***.Connection
***REMOVED***
***REMOVED***
        ***REMOVED***
***REMOVED***

            public SqlConn(String user, String pwd, String db)
            ***REMOVED***
    ***REMOVED***
                ***REMOVED***
                    String conn = $"Server=localhost\\SQLEXPRESS03;Database=***REMOVED***db***REMOVED***;Trusted_Connection=True;";
                    sqlCon = new SqlConnection(conn);
        ***REMOVED***catch (Exception e)
                ***REMOVED***
                    MessageBox.Show($"Ocurrio un error con la conexion!, codigo de error: \n***REMOVED***e***REMOVED***");
        ***REMOVED***
    ***REMOVED***

        public SqlConn(String db)
        ***REMOVED***
            String conn = $"Server=localhost\\SQLEXPRESS03;Database=***REMOVED***db***REMOVED***;Trusted_Connection=True;";
***REMOVED***
            ***REMOVED***
                sqlCon = new SqlConnection(conn);
    ***REMOVED***
***REMOVED***
            ***REMOVED***
                MessageBox.Show($"Ocurrio un error con la conexion!, codigo de error: \n***REMOVED***e***REMOVED***");
    ***REMOVED***
***REMOVED***
        
***REMOVED***
            ***REMOVED***
    ***REMOVED***
    ***REMOVED***

***REMOVED***
***REMOVED***
