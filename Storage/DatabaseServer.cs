using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uber.Storage
{
    class DatabaseServer
    {
        public string Hostname;
        public uint Port;

        public string Username;
        public string Password;

        public DatabaseServer(string _Hostname, uint _Port, string _Username, string _Password)
        {
            Hostname = _Hostname;
            Port = _Port;
            Username = _Username;
            Password = _Password;
        }
    }
}
