using System;
using System.Collections.Generic;
using Uber.HabboHotel.Misc;
using Uber.HabboHotel.Support;
using Uber.HabboHotel.Users;
using Uber.HabboHotel.Users.Authenticator;
using Uber.Messages;
using Uber.Net;
using Uber.Storage;
using Uber.Util;

namespace Uber.HabboHotel.GameClients
{
    class GameClient
    {
        private uint Id;

        private TcpConnection Connection;
        private GameClientMessageHandler MessageHandler;

        private Habbo Habbo;

        public Boolean PongOK;

        public uint ClientId
        {
            get
            {
                return Id;
            }
        }

        public Boolean LoggedIn
        {
            get
            {
                if (Habbo == null)
                {
                    return false;
                }

                return true;
            }
        }

        public GameClient(uint ClientId)
        {
            Id = ClientId;
            Connection = UberEnvironment.GetConnectionManager().GetConnection(ClientId);
            MessageHandler = new GameClientMessageHandler(this);
        }

        public TcpConnection GetConnection()
        {
            return Connection;
        }

        public GameClientMessageHandler GetMessageHandler()
        {
            return MessageHandler;
        }

        public Habbo GetHabbo()
        {
            return Habbo;
        }

        public void StartConnection()
        {
            if (Connection == null)
            {
                return;
            }

            PongOK = true;

            MessageHandler.RegisterGlobal();
            MessageHandler.RegisterHandshake();
            MessageHandler.RegisterHelp();

            TcpConnection.RouteReceivedDataCallback DataRouter = new TcpConnection.RouteReceivedDataCallback(HandleConnectionData);
            Connection.Start(DataRouter);
        }

        public void Login(string AuthTicket)
        {
            try
            {
                Habbo NewHabbo = Authenticator.TryLoginHabbo(AuthTicket);

                UberEnvironment.GetGame().GetClientManager().LogClonesOut(NewHabbo.Username);

                this.Habbo = NewHabbo;
                this.Habbo.LoadData();
            }
            catch (IncorrectLoginException e)
            {
                SendNotif("Login error: " + e.Message);
                Disconnect();

                return;
            }

            try
            {
                UberEnvironment.GetGame().GetBanManager().CheckForBanConflicts(this);
            }
            catch (ModerationBanException e)
            {
                SendBanMessage(e.Message);
                Disconnect();

                return;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE users SET online = '1', auth_ticket = '', ip_last = '" + GetConnection().IPAddress + "' WHERE id = '" + GetHabbo().Id + "' LIMIT 1");
                dbClient.ExecuteQuery("UPDATE user_info SET login_timestamp = '" + UberEnvironment.GetUnixTimestamp() + "' WHERE user_id = '" + GetHabbo().Id + "' LIMIT 1");
            }

            List<string> Rights = UberEnvironment.GetGame().GetRoleManager().GetRightsForHabbo(GetHabbo());

            ServerPacket _fuse = new ServerPacket(2);
            _fuse.AppendInt32(Rights.Count);

            foreach (string Right in Rights)
            {
                _fuse.AppendStringWithBreak(Right);
            }

            SendPacket(_fuse);

            if (GetHabbo().HasFuse("fuse_mod"))
            {
                SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeTool());
                UberEnvironment.GetGame().GetModerationTool().SendOpenTickets(this);
            }

            SendPacket(GetHabbo().GetAvatarEffectsInventoryComponent().Serialize());

            MessageHandler.GetResponse().Init(290);
            MessageHandler.GetResponse().AppendBoolean(true);
            MessageHandler.GetResponse().AppendBoolean(false);
            MessageHandler.SendResponse();

            MessageHandler.GetResponse().Init(3);
            MessageHandler.SendResponse();

            MessageHandler.GetResponse().Init(517);
            MessageHandler.GetResponse().AppendBoolean(true);
            MessageHandler.SendResponse();

            if (UberEnvironment.GetGame().GetPixelManager().NeedsUpdate(this))
            {
                UberEnvironment.GetGame().GetPixelManager().GivePixels(this);
            }

            MessageHandler.GetResponse().Init(455);
            MessageHandler.GetResponse().AppendUInt(GetHabbo().HomeRoom);
            MessageHandler.SendResponse();

            MessageHandler.GetResponse().Init(458);
            MessageHandler.GetResponse().AppendInt32(30);
            MessageHandler.GetResponse().AppendInt32(GetHabbo().FavoriteRooms.Count);

            foreach (uint Id in GetHabbo().FavoriteRooms)
            {
                MessageHandler.GetResponse().AppendUInt(Id);
            }

            MessageHandler.SendResponse();

            WelcomeMesage("Please change your welcome Message in GameClient.cs\n\n-PowahAlert");
            UberEnvironment.GetGame().GetAchievementManager().UnlockAchievement(this, 11, 1);

            MessageHandler.RegisterUsers();
            MessageHandler.RegisterMessenger();
            MessageHandler.RegisterNavigator();
            MessageHandler.RegisterRooms();
            
        }

        public void SendBanMessage(string Message)
        {
            ServerPacket BanMessage = new ServerPacket(35);
            BanMessage.AppendStringWithBreak("A Moderator has kicked you from Habbo Hotel:", 13);
            BanMessage.AppendStringWithBreak(Message);
            GetConnection().SendPacket(BanMessage);
        }

        public void SendNotif(string Message)
        {
            SendNotif(Message, false);
        }

        public void SendNotif(string Message, Boolean FromHotelManager)
        {
            ServerPacket nMessage = new ServerPacket();

            if (FromHotelManager)
            {
                nMessage.Init(139);
            }
            else
            {
                nMessage.Init(161);
            }

            nMessage.AppendStringWithBreak(Message);
            GetConnection().SendPacket(nMessage);
        }

        public void WelcomeMesage(string Message)
        {
            ServerPacket Welcome = new ServerPacket();
            Welcome.Init(810);
            Welcome.AppendUInt(1);
            Welcome.AppendStringWithBreak(Message);
            GetConnection().SendPacket(Welcome);
        }

        public void SendNotif(string Message, string Url)
        {
            ServerPacket nMessage = new ServerPacket(161);
            nMessage.AppendStringWithBreak(Message);
            nMessage.AppendStringWithBreak(Url);
            GetConnection().SendPacket(nMessage);
        }

        public void Stop()
        {
            if (GetHabbo() != null)
            {
                Habbo.OnDisconnect();
                Habbo = null;
            }

            if (GetConnection() != null)
            {
                Connection = null;
            }

            if (GetMessageHandler() != null)
            {
                MessageHandler.Destroy();
                MessageHandler = null;
            }
        }

        public void Disconnect()
        {
            UberEnvironment.GetGame().GetClientManager().StopClient(Id);
        }

        public void HandleConnectionData(ref byte[] data)
        {
            if (data[0] == 64)
            {
                int pos = 0;

                while (pos < data.Length)
                {
                    try
                    {
                        int MessageLength = Base64Encoding.DecodeInt32(new byte[] { data[pos++], data[pos++], data[pos++] });
                        uint MessageId = Base64Encoding.DecodeUInt32(new byte[] { data[pos++], data[pos++] });

                        byte[] Content = new byte[MessageLength - 2];

                        for (int i = 0; i < Content.Length; i++)
                        {
                            Content[i] = data[pos++];
                        }

                        ClientPacket Message = new ClientPacket(MessageId, Content);
                        MessageHandler.HandleRequest(Message);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        UberEnvironment.GetGame().GetClientManager().StopClient(Id);
                    }
                    catch (NullReferenceException)
                    {
                        UberEnvironment.GetGame().GetClientManager().StopClient(Id);
                    }
                    catch (EntryPointNotFoundException e)
                    {
                        UberEnvironment.GetLogging().WriteLine("User D/C: " + e.Message, Core.LogLevel.Error);
                        Disconnect();
                    }
                }
            }
            else
            {
                Connection.SendData(CrossdomainPolicy.GetXmlPolicy());
            }
        }

        public void SendPacket(ServerPacket _packet)
        {
            if (_packet != null)
            {
                GetConnection().SendPacket(_packet);
            }
        }
    }
}
