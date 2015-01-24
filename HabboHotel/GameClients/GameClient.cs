using System;
using System.Collections.Generic;
using Uber.Communication;
using Uber.HabboHotel.Misc;
using Uber.HabboHotel.Rooms;
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
        }

        public TcpConnection GetConnection()
        {
            return Connection;
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

            ServerPacket packet = new ServerPacket(290);
            packet.AppendBoolean(true);
            packet.AppendBoolean(false);
            SendPacket(packet);

            SendPacket(new ServerPacket(3));

            packet = new ServerPacket(517);
            packet.AppendBoolean(true);
            SendPacket(packet);

            if (UberEnvironment.GetGame().GetPixelManager().NeedsUpdate(this))
            {
                UberEnvironment.GetGame().GetPixelManager().GivePixels(this);
            }

            packet = new ServerPacket(455);
            packet.AppendUInt(GetHabbo().HomeRoom);
            SendPacket(packet);

            packet = new ServerPacket(458);
            packet.AppendInt32(30);
            packet.AppendInt32(GetHabbo().FavoriteRooms.Count);

            foreach (uint Id in GetHabbo().FavoriteRooms)
            {
                packet.AppendUInt(Id);
            }

            SendPacket(packet);

            WelcomeMesage("Please change your welcome Message in GameClient.cs\n\n-PowahAlert");
            UberEnvironment.GetGame().GetAchievementManager().UnlockAchievement(this, 11, 1);
            
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
                        int _len = Base64Encoding.DecodeInt32(new byte[] { data[pos++], data[pos++], data[pos++] });
                        uint _id = Base64Encoding.DecodeUInt32(new byte[] { data[pos++], data[pos++] });

                        byte[] _content = new byte[_len - 2];

                        for (int i = 0; i < _content.Length; i++)
                        {
                            _content[i] = data[pos++];
                        }

                        ClientPacket _packet = new ClientPacket(_id, _content);
                        IPacketEvent PacketEvent;
                        if (UberEnvironment.GetPacketManager().Handle(_packet.Id, out PacketEvent))
                        {
                            PacketEvent.parse(this, _packet);
                        }
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



        public void PrepareRoomForUser(uint Id, string Password)
        {
            ClearRoomLoading();

            if (UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id) == null)
            {
                return;
            }

            if (this.GetHabbo().InRoom)
            {
                Room OldRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(this.GetHabbo().CurrentRoomId);

                if (OldRoom != null)
                {
                    OldRoom.RemoveUserFromRoom(this, false, false);
                }
            }

            if (!UberEnvironment.GetGame().GetRoomManager().IsRoomLoaded(Id))
            {
                UberEnvironment.GetGame().GetRoomManager().LoadRoom(Id);
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Id);

            if (Room == null)
            {
                return;
            }

            this.GetHabbo().LoadingRoom = Id;

            if (Room.UserIsBanned(this.GetHabbo().Id))
            {
                if (Room.HasBanExpired(this.GetHabbo().Id))
                {
                    Room.RemoveBan(this.GetHabbo().Id);
                }
                else
                {
                    ServerPacket packet = new ServerPacket(224);
                    packet.AppendInt32(4);
                    SendPacket(packet);

                    SendPacket(new ServerPacket(18));

                    return;
                }
            }

            if (Room.UsersNow >= Room.UsersMax)
            {
                if (!UberEnvironment.GetGame().GetRoleManager().RankHasRight(this.GetHabbo().Rank, "fuse_enter_full_rooms"))
                {
                    ServerPacket packet = new ServerPacket(224);
                    packet.AppendInt32(1);
                    SendPacket(packet);

                    SendPacket(new ServerPacket(18));

                    return;
                }
            }

            if (Room.Type == "public")
            {
                if (Room.State > 0 && !this.GetHabbo().HasFuse("fuse_mod"))
                {
                    SendNotif("This public room is accessible to staff only.");

                    SendPacket(new ServerPacket(18));

                    return;
                }

                ServerPacket packet = new ServerPacket(166);
                packet.AppendStringWithBreak("/client/public/" + Room.ModelName + "/" + Room.RoomId);
                SendPacket(packet);
            }
            else if (Room.Type == "private")
            {
                SendPacket(new ServerPacket(19));

                if (!this.GetHabbo().HasFuse("fuse_enter_any_room") && !Room.CheckRights(this, true) && !this.GetHabbo().IsTeleporting)
                {
                    if (Room.State == 1)
                    {
                        if (Room.UserCount == 0)
                        {
                            SendPacket(new ServerPacket(131));
                        }
                        else
                        {
                            ServerPacket packet = new ServerPacket(91);
                            packet.AppendStringWithBreak("");
                            SendPacket(packet);

                            ServerPacket RingMessage = new ServerPacket(91);
                            RingMessage.AppendStringWithBreak(GetHabbo().Username);
                            Room.SendMessageToUsersWithRights(RingMessage);
                        }

                        return;
                    }
                    else if (Room.State == 2)
                    {
                        if (Password.ToLower() != Room.Password.ToLower())
                        {
                            ServerPacket packet = new ServerPacket(33);
                            packet.AppendInt32(-100002);
                            SendPacket(packet);

                            SendPacket(new ServerPacket(18));

                            return;
                        }
                    }
                }

                ServerPacket p166 = new ServerPacket(166);
                p166.AppendStringWithBreak("/client/private/" + Room.RoomId + "/id");
                SendPacket(p166);
            }

            this.GetHabbo().LoadingChecksPassed = true;

            LoadRoomForUser();
        }

        public void LoadRoomForUser()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(this.GetHabbo().LoadingRoom);

            if (Room == null || !this.GetHabbo().LoadingChecksPassed)
            {
                return;
            }

            // todo: Room.SerializeGroupBadges()
            ServerPacket packet = new ServerPacket(309);
            packet.AppendStringWithBreak("IcIrDs43103s19014d5a1dc291574a508bc80a64663e61a00");
            SendPacket(packet);

            packet = new ServerPacket(69);
            packet.AppendStringWithBreak(Room.ModelName);
            packet.AppendUInt(Room.RoomId);
            SendPacket(packet);

            if (this.GetHabbo().SpectatorMode)
            {
                SendPacket(new ServerPacket(254));
            }

            if (Room.Type == "private")
            {
                if (Room.Wallpaper != "0.0")
                {
                    ServerPacket wallpaper = new ServerPacket(46);
                    wallpaper.AppendStringWithBreak("wallpaper");
                    wallpaper.AppendStringWithBreak(Room.Wallpaper);
                    SendPacket(wallpaper);
                }

                if (Room.Floor != "0.0")
                {
                    ServerPacket floor = new ServerPacket(46);
                    floor.AppendStringWithBreak("floor");
                    floor.AppendStringWithBreak(Room.Floor);
                    SendPacket(floor);
                }

                ServerPacket landscape = new ServerPacket(46);
                landscape.AppendStringWithBreak("landscape");
                landscape.AppendStringWithBreak(Room.Landscape);
                SendPacket(landscape);

                if (Room.CheckRights(this, true))
                {
                    SendPacket(new ServerPacket(42));
                    SendPacket(new ServerPacket(47));
                }
                else if (Room.CheckRights(this))
                {
                    SendPacket(new ServerPacket(42));
                }

                ServerPacket p345 = new ServerPacket(345);

                if (this.GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.CheckRights(this, true))
                {
                    p345.AppendInt32(Room.Score);
                }
                else
                {
                    p345.AppendInt32(-1);
                }

                SendPacket(p345);

                if (Room.HasOngoingEvent)
                {
                    this.SendPacket(Room.Event.Serialize(this));
                }
                else
                {
                    ServerPacket p370 = new ServerPacket(370);
                    p370.AppendStringWithBreak("-1");
                    SendPacket(p370);
                }
            }
        }

        public void ClearRoomLoading()
        {
            GetHabbo().LoadingRoom = 0;
            GetHabbo().LoadingChecksPassed = false;
        }
    }
}
