***REMOVED***
using System.IO;

***REMOVED***.Connection
***REMOVED***
    public class LoadConfig
    ***REMOVED***
        public IConfiguration Configuration ***REMOVED*** get; ***REMOVED***

        public LoadConfig()
        ***REMOVED***
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
***REMOVED***
***REMOVED***
***REMOVED***