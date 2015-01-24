using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using MySql.Data.MySqlClient;

using Uber.Core;

namespace Uber.Storage
{
    class Database
    {
        public string DatabaseName;
        public uint PoolMinSize;
        public uint PoolMaxSize;

        public Database(string _DatabaseName, uint _PoolMinSize, uint _PoolMaxSize)
        {
            DatabaseName = _DatabaseName;

            PoolMinSize = _PoolMinSize;
            PoolMaxSize = _PoolMaxSize;
        }
    }
}
