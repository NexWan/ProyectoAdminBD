***REMOVED***
using System.Collections.Generic;
***REMOVED***
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

***REMOVED***.Connection
***REMOVED***
***REMOVED***
        ***REMOVED***
        private SqlConnection sqlCon;
            public SqlConn(String user, String pwd, String db)
            ***REMOVED***
                String conn = $"Server=localhost\\SQLEXPRESS03;Database=***REMOVED***db***REMOVED***;Trusted_Connection=True;";
                sqlCon = new SqlConnection(conn);
    ***REMOVED***

***REMOVED***
            ***REMOVED***
    ***REMOVED***
    ***REMOVED***

***REMOVED***
***REMOVED***
