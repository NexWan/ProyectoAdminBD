using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdminBD.Theme
{
/*
    This class will be a Singleton that is going to be holding all data that needs to be shared between windows, 
    might not be final but will be used while on development
*/
    internal class DataHolder
    {
        private static DataHolder instance;
        public string UserLoggedIn { get; private set; }
        public string userFirstName { get; private set; }
        public string userLastFName { get; private set; }
        public string userLastMName { get; private set; }   
        public string currentQuery { get; private set; }
        public Decimal userId { get; private set; }  
        public DataHolder() {
            this.UserLoggedIn = "";
        }

        public static DataHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataHolder();
                }
                return instance;
            }
        }

        public void ChangeUser(String name)
        {
            UserLoggedIn = name; 
        }

        public void ChangeUserFirstName(String name)
        {
            userFirstName = name;
        }

        public void SetUserLastFName(String name)
        {
            userLastFName = name;
        }

        public void setUserLastMName(String name)
        {
            userLastMName = name;
        }

        public void setUserId(Decimal id)
        {
            userId = id;
        }

        public void setCurrentQuery(string query)
        {
            this.currentQuery = query;
        }

        public bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
