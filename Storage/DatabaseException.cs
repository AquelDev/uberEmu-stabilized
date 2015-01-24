using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uber.Storage
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string sMessage) : base(sMessage) { }
    }
}
