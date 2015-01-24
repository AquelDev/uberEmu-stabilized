using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace Uber.Storage
{
    class DatabaseManager
    {
        public DatabaseServer Server;
        public Database Database;

        public string ConnectionString
        {
            get
            {
                MySqlConnectionStringBuilder ConnString = new MySqlConnectionStringBuilder();

                ConnString.Server = Server.Hostname;
                ConnString.Port = Server.Port;
                ConnString.UserID = Server.Username;
                ConnString.Password = Server.Password;
                ConnString.Database = Database.DatabaseName;
                ConnString.MinimumPoolSize = Database.PoolMinSize;
                ConnString.MaximumPoolSize = Database.PoolMaxSize;

                return ConnString.ToString();
            }
        }

        public DatabaseManager(DatabaseServer _Server, Database _Database)
        {
            Server = _Server;
            Database = _Database;
        }

        public DatabaseClient GetClient()
        {
            return new DatabaseClient(this);
        }
    }
}
