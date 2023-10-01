# Connection

### This folder at the moment has this 2 classes:
- LoadConfig.cs
- SqlConn.cs

## Load Config
LoadConfig class loads the configurations files that manages the connection string to the db (it was already exposed
but i mean, i can kinda make that habit of loading everything from secret files)
``` Cs
	Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
```
This section of code is the one that loads the settings so i can access the json info

## SqlConn.cs
This class just loads the json files that contains the necessary stuff to connect to the db and returns a sql connection
