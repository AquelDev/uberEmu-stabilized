﻿using System;
using System.Net;
using System.Net.Sockets;

using Uber.Core;

namespace Uber.Net
{
    class TcpConnectionListener
    {
        private TcpListener Listener;
        private Boolean IsListening;
        private AsyncCallback ConnectionReqCallback;

        private TcpConnectionManager Manager;
        private TcpConnectionFactory Factory;

        public Boolean isListening
        {
            get
            {
                return this.isListening;
            }
        }


        public TcpConnectionListener(string LocalIp, int Port, TcpConnectionManager Manager)
        {
            IPAddress IP = null;

            if (!IPAddress.TryParse(LocalIp, out IP))
            {
                IP = IPAddress.Loopback;
                UberEnvironment.GetLogging().WriteLine("[TCPListener.Construct]: Could not bind to " + LocalIp + ", binding to " + IPAddress.Loopback.ToString() + " instead.", LogLevel.Error);
            }

            this.Listener = new TcpListener(IP, Port);
            this.ConnectionReqCallback = new AsyncCallback(ConnectionRequest);
            this.Factory = new TcpConnectionFactory();
            this.Manager = Manager;
        }

        public void Start()
        {
            if (IsListening)
            {
                return;
            }

            Listener.Start(50);
            IsListening = true;

            WaitForNextConnection();
        }

        public void Stop()
        {
            if (!IsListening)
            {
                return;
            }

            IsListening = false;
            Listener.Stop();
        }

        public void Destroy()
        {
            try
            {
                Stop();

                Listener = null;
                Manager = null;
                Factory = null;
                GC.Collect();
            }
            catch
            {
                Console.WriteLine("Could not destroy the Listener,Manager, and Factory. Collecting Garbage - Failed.");
            }
            finally
            {
                Console.WriteLine("Destroyed The TcpConnectionListener Class. Collected Garbage Successfully.");
            }
        }

        private void WaitForNextConnection()
        {
            if (IsListening)
                Listener.BeginAcceptSocket(ConnectionReqCallback, null);
        }

        private void ConnectionRequest(IAsyncResult iAr)
        {
            try
            {
                Socket Sock = Listener.EndAcceptSocket(iAr);

                TcpConnection Connection = Factory.CreateConnection(Sock);

                if (Connection != null)
                {
                    Manager.HandleNewConnection(Connection);
                }
            }

            catch (Exception e)
            {
                if (IsListening)
                {
                    UberEnvironment.GetLogging().WriteLine("[TCPListener.OnRequest]: Could not handle new connection request: " + e.Message, LogLevel.Warning);
                }
            }

            finally
            {
                if (IsListening)
                {
                    WaitForNextConnection();
                }
            }
        }
    }
}
