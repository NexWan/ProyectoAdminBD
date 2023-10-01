# Welcome
This is the root folder of the project where all cs classes are being created bla bla bla.
app.xaml sets up the main scene and some resources and <b>proyectoadminbd.csproj</b> is the one that loads all configuration of the project

## Login.xaml
This xaml file contains all the GUI code in xaml

## Login.xaml.cs
This class contains all the logic behind the login window, one of the most important factors is how to create the connection

### Creating the connection to the DB
```cs
private IConfiguration _configuration;

        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var appConfig = new LoadConfig();
            _configuration = appConfig.Configuration;
        }
```
This snippet code loads the configuration onto a IConfiguration type that will be pass on to the connection class

``` cs
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
```
As you can see, this is the SqlCon class, where we need a IConfiguration type to access the database connectionString, so in order to create a connection we'll need this code snippet:

```cs
SqlConnection? conn = null;
            try
            {
                conn = new SqlConn(_configuration).GetConnection();
                conn.Open();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
```
And as easy as that you are connected to the sql server instance! this can be replicated to all the cs classes that requires for a db connection!
