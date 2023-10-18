using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
