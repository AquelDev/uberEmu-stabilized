using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;

using Uber.Messages;
using Uber.Storage;
using Uber.HabboHotel.Support;

namespace Uber.HabboHotel.GameClients
{
    partial class GameClientManager
    {
        private Thread ConnectionChecker;
        private Dictionary<uint, GameClient> Clients;

        public int ClientCount
        {
            get
            {
                return this.Clients.Count;
            }
        }

        public GameClientManager()
        {
            this.Clients = new Dictionary<uint, GameClient>();
        }

        public void Clear()
        {
            Clients.Clear();
        }

        public GameClient GetClient(uint ClientId)
        {
            if (Clients.ContainsKey(ClientId))
            {
                return Clients[ClientId];
            }

            return null;
        }

        public bool RemoveClient(uint ClientId)
        {
            return Clients.Remove(ClientId);
        }

        public void StartClient(uint ClientId)
        {
            Clients.Add(ClientId, new GameClient(ClientId));
            Clients[ClientId].StartConnection();
        }

        public void StopClient(uint ClientId)
        {
            GameClient Client = GetClient(ClientId);

            if (Client == null)
            {
                return;
            }

            UberEnvironment.GetConnectionManager().DropConnection(ClientId);

            Client.Stop();
            RemoveClient(ClientId);
        }

        public void StartConnectionChecker()
        {
            if (ConnectionChecker != null)
            {
                return;
            }

            ConnectionChecker = new Thread(TestClientConnections);
            ConnectionChecker.Name = "Connection Checker";
            ConnectionChecker.Priority = ThreadPriority.Lowest;
            ConnectionChecker.Start();
        }

        public void StopConnectionChecker()
        {
            if (ConnectionChecker == null)
            {
                return;
            }

            try
            {
                ConnectionChecker.Abort();
            }
            catch (ThreadAbortException) { }

            ConnectionChecker = null;
        }

        private void TestClientConnections()
        {
            int interval = int.Parse(UberEnvironment.GetConfig().data["client.ping.interval"]);

            if (interval <= 1000)
            {
                throw new ArgumentException("Invalid configuration value for ping interval! Must be above 1000 miliseconds.");
            }

            while (true)
            {
                try
                {
                    ServerPacket PingMessage = new ServerPacket(50);
                    List<uint> TimedOutClients = new List<uint>();
                    foreach (var _client in Clients)
                    {
                        if (_client.Value.PongOK)
                        {
                            _client.Value.PongOK = false;
                            _client.Value.GetConnection().SendPacket(PingMessage);
                        }
                        else
                        {
                            UberEnvironment.GetLogging().WriteLine(_client.Value.ClientId + " has timed out.", Core.LogLevel.Warning);
                            TimedOutClients.Add(_client.Value.ClientId);
                        }
                    }

                    foreach (uint ClientId in TimedOutClients)
                    {
                        StopClient(ClientId);
                    }


                    TimedOutClients.Clear();
                    Thread.Sleep(interval);
                    UberEnvironment.GetLogging().WriteLine("[Info] Checking Pings", Core.LogLevel.Warning);
                }
                catch (ThreadAbortException)
                {
                    UberEnvironment.GetLogging().WriteLine("Ping Thread has aborted unexpectedly.", Core.LogLevel.Error);
                    StopConnectionChecker();
                    StartConnectionChecker();
                }
                catch (InvalidOperationException)
                {

                    UberEnvironment.GetLogging().WriteLine("Invalid Operation Exception on Ping Thread.", Core.LogLevel.Error);
                    StopConnectionChecker();
                    StartConnectionChecker();
                }
            }
        }

        public GameClient GetClientByHabbo(uint HabboId)
        {
            foreach (var _client in Clients)
            {
                if (_client.Value.GetHabbo() == null)
                {
                    continue;
                }
                if (_client.Value.GetHabbo().Id == HabboId)
                {
                    return _client.Value;
                }
            }
            return null;
        }

        public GameClient GetClientByHabbo(string Name)
        {
            foreach (var _client in Clients)
            {
                if (_client.Value.GetHabbo() == null)
                {
                    continue;
                }
                if (_client.Value.GetHabbo().Username.ToLower() == Name.ToLower())
                {
                    return _client.Value;
                }
            }
            return null;
        }

        public void BroadcastMessage(ServerPacket Message)
        {
            this.BroadcastMessage(Message, "");
        }

        public void BroadcastMessage(ServerPacket Message, string FuseRequirement)
        {
            foreach (var _client in Clients)
            {
                try
                {
                    if (FuseRequirement.Length > 0)
                    {
                        if (_client.Value.GetHabbo() == null || !_client.Value.GetHabbo().HasFuse(FuseRequirement))
                        {
                            continue;
                        }
                    }
                    _client.Value.SendPacket(Message);
                }
                catch (Exception) { }
            }
        }

        public void CheckEffects()
        {
            foreach (var _client in Clients)
            {
                if (_client.Value.GetHabbo() == null || _client.Value.GetHabbo().GetAvatarEffectsInventoryComponent() == null)
                {
                    continue;
                }
                _client.Value.GetHabbo().GetAvatarEffectsInventoryComponent().CheckExpired();
            }
        }
    }
}