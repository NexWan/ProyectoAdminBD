using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProyectoAdminBD.Connection
{
    public class LoadConfig
    {
        public IConfiguration Configuration { get; }

        public LoadConfig()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}