using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uber.Net
{
    class TcpConnectionManager
    {
        private Dictionary<uint, TcpConnection> Connections;
        private TcpConnectionListener Listener;

        public int AmountOfActiveConnections
        {
            get
            {
                return Connections.Count;
            }
        }

        public TcpConnectionManager(string LocalIP, int Port, int maxConnections)
        {
            Connections = new Dictionary<uint, TcpConnection>(maxConnections);
            Listener = new TcpConnectionListener(LocalIP, Port, this);
        }

        public void DestroyManager()
        {
            Connections.Clear();
            Connections = null;
            Listener = null;
        }

        public Boolean ContainsConnection(uint Id)
        {
            return Connections.ContainsKey(Id);
        }

        public TcpConnection GetConnection(uint Id)
        {
            if (Connections.ContainsKey(Id))
            {
                return Connections[Id];
            }

            return null;
        }

        public TcpConnectionListener GetListener()
        {
            return Listener;
        }

        public void HandleNewConnection(TcpConnection connection)
        {
            Connections.Add(connection.Id, connection);
            UberEnvironment.GetGame().GetClientManager().StartClient(connection.Id);
        }

        public void DropConnection(uint Id)
        {
            TcpConnection Connection = GetConnection(Id);

            if (Connection == null)
            {
                return;
            }

            Connection.GetSocket().Close();
            Connections.Remove(Id);
        }

        public Boolean VerifyConnection(uint Id)
        {
            TcpConnection Connection = GetConnection(Id);

            if (Connection != null)
            {
                return Connection.TestConnection();
            }

            return false;
        }
    }
}
