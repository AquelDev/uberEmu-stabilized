using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.GameClients;
using Uber.Messages;
using Uber.Storage;

namespace Uber.HabboHotel.Support
{
    class ModerationTool
    {
        #region General

        private List<SupportTicket> Tickets;

        public List<string> UserMessagePresets;
        public List<string> RoomMessagePresets;

        public ModerationTool()
        {
            Tickets = new List<SupportTicket>();
            UserMessagePresets = new List<string>();
            RoomMessagePresets = new List<string>();
        }

        public ServerPacket SerializeTool()
        {
            ServerPacket Message = new ServerPacket(531);
            Message.AppendInt32(-1);
            Message.AppendInt32(UserMessagePresets.Count);

            foreach (String Preset in UserMessagePresets)
            {
                Message.AppendStringWithBreak(Preset);
            }

            Message.AppendUInt(6); // Amount of Mod Actions

            Message.AppendStringWithBreak("Room Problems"); // modaction Cata
            Message.AppendUInt(8); // ModAction Count
            Message.AppendStringWithBreak("Door Blocking"); // mod action Cata
            Message.AppendStringWithBreak("Please stop blocking the doors."); // Msg
            Message.AppendStringWithBreak("Ban Last Warning"); // Mod Action Cata
            Message.AppendStringWithBreak("This is your last warning or you will be banned."); // Msg
            Message.AppendStringWithBreak("Player Support Issue");// Mod Action Cata
            Message.AppendStringWithBreak("Please contact player support for this issue, thank you."); // Msg
            Message.AppendStringWithBreak("Filter Bypass"); // Mod Cata
            Message.AppendStringWithBreak("Please stop bypassing the filter, this is your only warning."); // Msg
            Message.AppendStringWithBreak("Kick Abuse"); // Mod Cata
            Message.AppendStringWithBreak("Please stop kicking users without a valid reason, thank you."); // Msg
            Message.AppendStringWithBreak("Ban Room"); // Mod Cata
            Message.AppendStringWithBreak("Please stop banning people without a good reason"); // Msg
            Message.AppendStringWithBreak("Unacceptable Room Name"); // Mod Cata
            Message.AppendStringWithBreak("Your room name is unacceptable, please change it or expect further action."); // Msg
            Message.AppendStringWithBreak("Items not loading"); // Mod Cata
            Message.AppendStringWithBreak("Please contact an administrator for this issue."); // Msg

            Message.AppendStringWithBreak("Player Support");// modaction Cata
            Message.AppendUInt(8); // ModAction Count
            Message.AppendStringWithBreak("Bug Report"); // mod action Cata
            Message.AppendStringWithBreak("Thanks for reporting this bug, we appreciate your help."); // Msg
            Message.AppendStringWithBreak("Login Problem"); // Mod Action Cata
            Message.AppendStringWithBreak("We will contact an administrator and work on fixing your issue."); // Msg
            Message.AppendStringWithBreak("Help Support");// Mod Action Cata
            Message.AppendStringWithBreak("Please Contact The Player Support For this problem"); // Msg
            Message.AppendStringWithBreak("Call for Help Misuse"); // Mod Cata
            Message.AppendStringWithBreak("Please do not abuse the Call for Help, it is for genuine use only."); // Msg
            Message.AppendStringWithBreak("Privacy"); // Mod Cata
            Message.AppendStringWithBreak("Please do not give out your personal details."); // Msg
            Message.AppendStringWithBreak("Technical Issue"); // Mod Cata
            Message.AppendStringWithBreak("Please contact an administrator with your technical issue."); // Msg
            Message.AppendStringWithBreak("Cache Issue"); // Mod Cata
            Message.AppendStringWithBreak("Please reload your client and try again, thank you."); // Msg
            Message.AppendStringWithBreak("Other Issues"); // Mod Cata
            Message.AppendStringWithBreak("Please reload your client and try again, thank you."); // Msg

            Message.AppendStringWithBreak("Users Problems");// modaction Cata
            Message.AppendUInt(8); // ModAction Count
            Message.AppendStringWithBreak("Scamming"); // mod action Cata
            Message.AppendStringWithBreak("We cannot monitor scamming within the hotel, please becareful next time."); // Msg
            Message.AppendStringWithBreak("Trading Scam"); // Mod Action Cata
            Message.AppendStringWithBreak("We cannot monitor trading within the hotel, please becareful next time."); // Msg
            Message.AppendStringWithBreak("Disconnection");// Mod Action Cata
            Message.AppendStringWithBreak("Please contact an administrator with your issue."); // Msg
            Message.AppendStringWithBreak("Room Blocking"); // Mod Cata
            Message.AppendStringWithBreak("Please send a Call for Help with the users name, thank you."); // Msg
            Message.AppendStringWithBreak("Freezing"); // Mod Cata
            Message.AppendStringWithBreak("Please reload the client and try again, thank you."); // Msg
            Message.AppendStringWithBreak("Website Issue"); // Mod Cata
            Message.AppendStringWithBreak("Please contact an administrator with the error, thank you."); // Msg
            Message.AppendStringWithBreak("Login Issue"); // Mod Cata
            Message.AppendStringWithBreak("Please say the users name and we will look into it, thank you."); // Msg
            Message.AppendStringWithBreak("Updates"); // Mod Cata
            Message.AppendStringWithBreak("We always try to maintain the best updates."); // Msg

            Message.AppendStringWithBreak("Debug Problems");// modaction Cata
            Message.AppendUInt(8); // ModAction Count
            Message.AppendStringWithBreak("Lag"); // mod action Cata
            Message.AppendStringWithBreak("We are now looking into this issue, thank you."); // Msg
            Message.AppendStringWithBreak("Disconnection"); // Mod Action Cata
            Message.AppendStringWithBreak("What happens and how happens please explain us"); // Msg
            Message.AppendStringWithBreak("SSO Problem");// Mod Action Cata
            Message.AppendStringWithBreak("Please logout of the website and then log back in, thank you."); // Msg
            Message.AppendStringWithBreak("Char Filter"); // Mod Cata
            Message.AppendStringWithBreak("Can you say the users name and explain to us what happens."); // Msg
            Message.AppendStringWithBreak("System check"); // Mod Cata
            Message.AppendStringWithBreak("We are already looking into this issue, thank you."); // Msg
            Message.AppendStringWithBreak("Missing Feature"); // Mod Cata
            Message.AppendStringWithBreak("We are working on this feature sometime into the near future, thank you."); // Msg
            Message.AppendStringWithBreak("Feature Error"); // Mod Cata
            Message.AppendStringWithBreak("We are working on fixing this issue, thank you."); // Msg
            Message.AppendStringWithBreak("Flash Player Issue"); // Mod Cata
            Message.AppendStringWithBreak("We recommend you try installing flash player again, thank you."); // Msg

            Message.AppendStringWithBreak("Unused Category"); // Category
            Message.AppendUInt(8); // Amount of Sub-category
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message

            Message.AppendStringWithBreak("Unused Category"); // Category
            Message.AppendUInt(8); // Amount of Sub-category
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message
            Message.AppendStringWithBreak("Example Category"); // Sub-category
            Message.AppendStringWithBreak("This is an example message."); // Preset message

            Message.AppendStringWithBreak("");
            Message.AppendStringWithBreak("");
            Message.AppendStringWithBreak("");
            Message.AppendStringWithBreak("");
            Message.AppendStringWithBreak("");
            Message.AppendStringWithBreak("");
            Message.AppendStringWithBreak("");

            Message.AppendInt32(RoomMessagePresets.Count);

            foreach (String Preset in RoomMessagePresets)
            {
                Message.AppendStringWithBreak(Preset);
            }
            Message.AppendStringWithBreak("");
            return Message;
        }

        #endregion

        #region Message Presets

        public void LoadMessagePresets()
        {
            UserMessagePresets.Clear();
            RoomMessagePresets.Clear();

            DataTable Data = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Data = dbClient.ReadDataTable("SELECT type,message FROM moderation_presets WHERE enabled = '1'");
            }

            if (Data == null)
            {
                return;
            }

            foreach (DataRow Row in Data.Rows)
            {
                String Message = (String)Row["message"];

                switch (Row["type"].ToString().ToLower())
                {
                    case "message":

                        UserMessagePresets.Add(Message);
                        break;

                    case "roommessage":

                        RoomMessagePresets.Add(Message);
                        break;
                }
            }
        }

        #endregion

        #region Support Tickets

        public void LoadPendingTickets()
        {
            Tickets.Clear();

            DataTable Data = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Data = dbClient.ReadDataTable("SELECT id,score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp FROM moderation_tickets WHERE status = 'open' OR status = 'picked'");
            }

            if (Data == null)
            {
                return;
            }

            foreach (DataRow Row in Data.Rows)
            {
                SupportTicket Ticket = new SupportTicket((uint)Row["id"], (int)Row["score"], (int)Row["type"], (uint)Row["sender_id"], (uint)Row["reported_id"], (String)Row["message"], (uint)Row["room_id"], (String)Row["room_name"], (Double)Row["timestamp"]);

                if (Row["status"].ToString().ToLower() == "picked")
                {
                    Ticket.Pick((uint)Row["moderator_id"], false);
                }

                Tickets.Add(Ticket);
            }
        }

        public void SendNewTicket(GameClient Session, int Category, uint ReportedUser, String Message)
        {
            if (Session.GetHabbo().CurrentRoomId <= 0)
            {
                return;
            }

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Session.GetHabbo().CurrentRoomId);

            uint TicketId = 0;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("message", Message);
                dbClient.AddParamWithValue("name", Data.Name);

                dbClient.ExecuteQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + Category + "','open','" + Session.GetHabbo().Id + "','" + ReportedUser + "','0',@message,'" + Data.Id + "',@name,'" + UberEnvironment.GetUnixTimestamp() + "')");
                dbClient.ExecuteQuery("UPDATE user_info SET cfhs = cfhs + 1 WHERE user_id = '" + Session.GetHabbo().Id + "' LIMIT 1");

                TicketId = (uint)dbClient.ReadDataRow("SELECT id FROM moderation_tickets WHERE sender_id = '" + Session.GetHabbo().Id + "' ORDER BY id DESC LIMIT 1")[0];
            }

            SupportTicket Ticket = new SupportTicket(TicketId, 1, Category, Session.GetHabbo().Id, ReportedUser, Message, Data.Id, Data.Name, UberEnvironment.GetUnixTimestamp());

            Tickets.Add(Ticket);

            SendTicketToModerators(Ticket);
        }

        public void SendOpenTickets(GameClient Session)
        {
            foreach (SupportTicket Ticket in Tickets)
            {
                if (Ticket.Status != TicketStatus.OPEN && Ticket.Status != TicketStatus.PICKED)
                {
                    continue;
                }

                Session.SendPacket(Ticket.Serialize());
            }
        }

        public SupportTicket GetTicket(uint TicketId)
        {
            foreach (SupportTicket Ticket in Tickets)
            {
                if (Ticket.TicketId == TicketId)
                {
                    return Ticket;
                }
            }
            return null;
        }

        public void PickTicket(GameClient Session, uint TicketId)
        {
            SupportTicket Ticket = GetTicket(TicketId);

            if (Ticket == null || Ticket.Status != TicketStatus.OPEN)
            {
                return;
            }

            Ticket.Pick(Session.GetHabbo().Id, true);
            SendTicketToModerators(Ticket);
        }

        public void ReleaseTicket(GameClient Session, uint TicketId)
        {
            SupportTicket Ticket = GetTicket(TicketId);

            if (Ticket == null || Ticket.Status != TicketStatus.PICKED || Ticket.ModeratorId != Session.GetHabbo().Id)
            {
                return;
            }

            Ticket.Release(true);
            SendTicketToModerators(Ticket);
        }

        public void CloseTicket(GameClient Session, uint TicketId, int Result)
        {
            SupportTicket Ticket = GetTicket(TicketId);

            if (Ticket == null || Ticket.Status != TicketStatus.PICKED || Ticket.ModeratorId != Session.GetHabbo().Id)
            {
                return;
            }

            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Ticket.SenderId);

            TicketStatus NewStatus;
            int ResultCode;

            switch (Result)
            {
                case 1:

                    ResultCode = 1;
                    NewStatus = TicketStatus.INVALID;
                    break;

                case 2:

                    ResultCode = 2;
                    NewStatus = TicketStatus.ABUSIVE;

                    using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                    {
                        dbClient.ExecuteQuery("UPDATE user_info SET cfhs_abusive = cfhs_abusive + 1 WHERE user_id = '" + Ticket.SenderId + "' LIMIT 1");
                    }

                    break;

                case 3:
                default:

                    ResultCode = 0;
                    NewStatus = TicketStatus.RESOLVED;
                    break;
            }

            if (Client != null)
            {
                Client.GetMessageHandler().GetResponse().Init(540);
                Client.GetMessageHandler().GetResponse().AppendInt32(ResultCode);
                Client.GetMessageHandler().SendResponse();
            }

            Ticket.Close(NewStatus, true);
            SendTicketToModerators(Ticket);
        }

        public Boolean UsersHasPendingTicket(uint Id)
        {
            foreach (SupportTicket Ticket in Tickets)
            {
                if (Ticket.SenderId == Id && Ticket.Status == TicketStatus.OPEN)
                {
                    return true;
                }
            }
            return false;
        }

        public void DeletePendingTicketForUser(uint Id)
        {
            foreach (SupportTicket Ticket in Tickets)
            {
                if (Ticket.SenderId == Id)
                {
                    Ticket.Delete(true);
                    SendTicketToModerators(Ticket);
                    return;
                }
            }
        }

        public void SendTicketToModerators(SupportTicket Ticket)
        {
            UberEnvironment.GetGame().GetClientManager().BroadcastMessage(Ticket.Serialize(), "fuse_mod");
        }

        #endregion

        #region Room Moderation

        public void PerformRoomAction(GameClient ModSession, uint RoomId, Boolean KickUsers, Boolean LockRoom, Boolean InappropriateRoom)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);

            if (Room == null)
            {
                return;
            }

            if (LockRoom)
            {
                Room.State = 1;

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE rooms SET state = 'locked' WHERE id = '" + Room.RoomId + "' LIMIT 1");
                }
            }

            if (InappropriateRoom)
            {
                Room.Name = "Inappropriate to Hotel Managament";
                Room.Description = "Inappropriate to Hotel Management";
                Room.Tags.Clear();

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE rooms SET caption = 'Inappropriate to Hotel Management', description = 'Inappropriate to Hotel Management', tags = '' WHERE id = '" + Room.RoomId + "' LIMIT 1");
                }
            }

            if (KickUsers)
            {
                List<RoomUser> ToRemove = new List<RoomUser>();

                foreach (RoomUser User in Room.UserList)
                {
                    if (User.IsBot || User.GetClient().GetHabbo().Rank >= ModSession.GetHabbo().Rank)
                    {
                        continue;
                    }

                    ToRemove.Add(User);
                }

                for (int i = 0; i < ToRemove.Count; i++)
                {
                    Room.RemoveUserFromRoom(ToRemove[i].GetClient(), true, false);
                }
            }
        }

        public void RoomAlert(uint RoomId, Boolean Caution, String Message)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);

            if (Room == null || Message.Length <= 1)
            {
                return;
            }

            StringBuilder QueryBuilder = new StringBuilder();
            int j = 0;

            foreach (RoomUser User in Room.UserList)
            {
                if (User.IsBot)
                {
                    continue;
                }

                User.GetClient().SendNotif(Message, Caution);

                if (j > 0)
                {
                    QueryBuilder.Append(" OR ");
                }

                QueryBuilder.Append("user_id = '" + User.GetClient().GetHabbo().Id + "'");
                j++;
            }

            if (Caution)
            {
                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE user_info SET cautions = cautions + 1 WHERE " + QueryBuilder.ToString() + " LIMIT " + j);
                }

            }
        }

        public ServerPacket SerializeRoomTool(RoomData Data)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Data.Id);
            uint OwnerId = 0;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                try
                {
                    OwnerId = (uint)dbClient.ReadDataRow("SELECT id FROM users WHERE username = '" + Data.Owner + "' LIMIT 1")[0];
                }
                catch (Exception) { }
            }

            ServerPacket Message = new ServerPacket(538);
            Message.AppendUInt(Data.Id);
            Message.AppendInt32(Data.UsersNow); // user count

            if (Room != null)
            {
                Message.AppendBoolean((Room.GetRoomUserByHabbo(Data.Owner) != null));
            }
            else
            {
                Message.AppendBoolean(false);
            }

            Message.AppendUInt(OwnerId);
            Message.AppendStringWithBreak(Data.Owner);
            Message.AppendUInt(Data.Id);
            Message.AppendStringWithBreak(Data.Name);
            Message.AppendStringWithBreak(Data.Description);
            Message.AppendInt32(Data.TagCount);

            foreach (string Tag in Data.Tags)
            {
                Message.AppendStringWithBreak(Tag);
            }

            if (Room != null)
            {
                Message.AppendBoolean(Room.HasOngoingEvent);

                if (Room.Event != null)
                {
                    Message.AppendStringWithBreak(Room.Event.Name);
                    Message.AppendStringWithBreak(Room.Event.Description);
                    Message.AppendInt32(Room.Event.Tags.Count);

                    foreach (string Tag in Room.Event.Tags)
                    {
                        Message.AppendStringWithBreak(Tag);
                    }
                }
            }
            else
            {
                Message.AppendBoolean(false);
            }

            return Message;
        }

        #endregion

        #region User Moderation

        public void KickUser(GameClient ModSession, uint UserId, String Message, Boolean Soft)
        {
            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(UserId);

            if (Client == null || Client.GetHabbo().CurrentRoomId < 1 || Client.GetHabbo().Id == ModSession.GetHabbo().Id)
            {
                return;
            }

            if (Client.GetHabbo().Rank >= ModSession.GetHabbo().Rank)
            {
                ModSession.SendNotif("You do not have permission to kick that user.");
                return;
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Client.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            Room.RemoveUserFromRoom(Client, true, false);

            if (!Soft)
            {
                Client.SendNotif(Message);

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE user_info SET cautions = cautions + 1 WHERE user_id = '" + UserId + "' LIMIT 1");
                }
            }
        }

        public void AlertUser(GameClient ModSession, uint UserId, String Message, Boolean Caution)
        {
            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(UserId);

            if (Client == null || Client.GetHabbo().Id == ModSession.GetHabbo().Id)
            {
                return;
            }

            if (Caution && Client.GetHabbo().Rank >= ModSession.GetHabbo().Rank)
            {
                ModSession.SendNotif("You do not have permission to caution that user, sending as a regular message instead.");
                Caution = false;
            }

            Client.SendNotif(Message, Caution);

            if (Caution)
            {
                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE user_info SET cautions = cautions + 1 WHERE user_id = '" + UserId + "' LIMIT 1");
                }
            }
        }

        public void BanUser(GameClient ModSession, uint UserId, int Length, String Message)
        {
            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(UserId);

            if (Client == null || Client.GetHabbo().Id == ModSession.GetHabbo().Id)
            {
                return;
            }

            if (Client.GetHabbo().Rank >= ModSession.GetHabbo().Rank)
            {
                ModSession.SendNotif("You do not have permission to ban that user.");
                return;
            }

            Double dLength = Length;

            UberEnvironment.GetGame().GetBanManager().BanUser(Client, ModSession.GetHabbo().Username, dLength, Message, false);
        }

        #endregion

        #region User Info

        public ServerPacket SerializeUserInfo(uint UserId)
        {
            DataRow User = null;
            DataRow Info = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                User = dbClient.ReadDataRow("SELECT * FROM users WHERE id = '" + UserId + "' LIMIT 1");
                Info = dbClient.ReadDataRow("SELECT * FROM user_info WHERE user_id = '" + UserId + "' LIMIT 1");
            }

            if (User == null)
            {
                throw new ArgumentException();
            }

            ServerPacket Message = new ServerPacket(533);

            Message.AppendUInt((uint)User["id"]);
            Message.AppendStringWithBreak((string)User["username"]);

            if (Info != null)
            {
                Message.AppendInt32((int)Math.Ceiling((UberEnvironment.GetUnixTimestamp() - (Double)Info["reg_timestamp"]) / 60));
                Message.AppendInt32((int)Math.Ceiling((UberEnvironment.GetUnixTimestamp() - (Double)Info["login_timestamp"]) / 60));
            }
            else
            {
                Message.AppendInt32(0);
                Message.AppendInt32(0);
            }

            if (User["online"].ToString() == "1")
            {
                Message.AppendBoolean(true);
            }
            else
            {
                Message.AppendBoolean(false);
            }

            if (Info != null)
            {
                Message.AppendInt32((int)Info["cfhs"]);
                Message.AppendInt32((int)Info["cfhs_abusive"]);
                Message.AppendInt32((int)Info["cautions"]);
                Message.AppendInt32((int)Info["bans"]);
            }
            else
            {
                Message.AppendInt32(0); // cfhs
                Message.AppendInt32(0); // abusive cfhs
                Message.AppendInt32(0); // cautions
                Message.AppendInt32(0); // bans
            }

            return Message;
        }

        public ServerPacket SerializeRoomVisits(uint UserId)
        {
            DataTable Data = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Data = dbClient.ReadDataTable("SELECT room_id,hour,minute FROM user_roomvisits WHERE user_id = '" + UserId + "' ORDER BY entry_timestamp DESC LIMIT 50");
            }

            ServerPacket Message = new ServerPacket(537);
            Message.AppendUInt(UserId);
            Message.AppendStringWithBreak(UberEnvironment.GetGame().GetClientManager().GetNameById(UserId));

            if (Data != null)
            {
                Message.AppendInt32(Data.Rows.Count);

                foreach (DataRow Row in Data.Rows)
                {
                    RoomData RoomData = UberEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData((uint)Row["room_id"]);

                    Message.AppendBoolean(RoomData.IsPublicRoom);
                    Message.AppendUInt(RoomData.Id);
                    Message.AppendStringWithBreak(RoomData.Name);
                    Message.AppendInt32((int)Row["hour"]);
                    Message.AppendInt32((int)Row["minute"]);
                }
            }
            else
            {
                Message.AppendInt32(0);
            }

            return Message;
        }

        #endregion

        #region Chatlogs

        public ServerPacket SerializeUserChatlog(uint UserId)
        {
            DataTable Visits = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Visits = dbClient.ReadDataTable("SELECT room_id,entry_timestamp,exit_timestamp FROM user_roomvisits WHERE user_id = '" + UserId + "' ORDER BY entry_timestamp DESC LIMIT 5");
            }

            ServerPacket Message = new ServerPacket(536);
            Message.AppendUInt(UserId);
            Message.AppendStringWithBreak(UberEnvironment.GetGame().GetClientManager().GetNameById(UserId));

            if (Visits != null)
            {
                Message.AppendInt32(Visits.Rows.Count);

                foreach (DataRow Visit in Visits.Rows)
                {
                    DataTable Chatlogs = null;

                    if ((Double)Visit["exit_timestamp"] <= 0.0)
                    {
                        Visit["exit_timestamp"] = UberEnvironment.GetUnixTimestamp();
                    }

                    using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                    {
                        Chatlogs = dbClient.ReadDataTable("SELECT user_id,user_name,hour,minute,message FROM chatlogs WHERE room_id = '" + (uint)Visit["room_id"] + "' AND timestamp > '" + (Double)Visit["entry_timestamp"] + "' AND timestamp < '" + (Double)Visit["exit_timestamp"] + "' ORDER BY timestamp DESC");
                    }

                    RoomData RoomData = UberEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData((uint)Visit["room_id"]);

                    Message.AppendBoolean(RoomData.IsPublicRoom);
                    Message.AppendUInt(RoomData.Id);
                    Message.AppendStringWithBreak(RoomData.Name);

                    if (Chatlogs != null)
                    {
                        Message.AppendInt32(Chatlogs.Rows.Count);

                        foreach (DataRow Log in Chatlogs.Rows)
                        {
                            Message.AppendInt32((int)Log["hour"]);
                            Message.AppendInt32((int)Log["minute"]);
                            Message.AppendUInt((uint)Log["user_id"]);
                            Message.AppendStringWithBreak((string)Log["user_name"]);
                            Message.AppendStringWithBreak((string)Log["message"]);
                        }
                    }
                    else
                    {
                        Message.AppendInt32(0);
                    }
                }
            }
            else
            {
                Message.AppendInt32(0);
            }

            return Message;
        }

        public ServerPacket SerializeTicketChatlog(SupportTicket Ticket, RoomData RoomData, Double Timestamp)
        {
            DataTable Data = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Data = dbClient.ReadDataTable("SELECT user_id,user_name,hour,minute,message FROM chatlogs WHERE room_id = '" + RoomData.Id + "' AND timestamp >= '" + (Timestamp - 300) + "' AND timestamp <= '" + Timestamp + "' ORDER BY timestamp DESC");
            }

            ServerPacket Message = new ServerPacket(534);
            Message.AppendUInt(Ticket.TicketId);
            Message.AppendUInt(Ticket.SenderId);
            Message.AppendUInt(Ticket.ReportedId);
            Message.AppendBoolean(RoomData.IsPublicRoom);
            Message.AppendUInt(RoomData.Id);
            Message.AppendStringWithBreak(RoomData.Name);

            if (Data != null)
            {
                Message.AppendInt32(Data.Rows.Count);

                foreach (DataRow Row in Data.Rows)
                {
                    Message.AppendInt32((int)Row["hour"]);
                    Message.AppendInt32((int)Row["minute"]);
                    Message.AppendUInt((uint)Row["user_id"]);
                    Message.AppendStringWithBreak((String)Row["user_name"]);
                    Message.AppendStringWithBreak((String)Row["message"]);
                }
            }
            else
            {
                Message.AppendInt32(0);
            }

            return Message;
        }

        public ServerPacket SerializeRoomChatlog(uint RoomId)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);

            if (Room == null)
            {
                throw new ArgumentException();
            }

            Boolean IsPublic = false;

            if (Room.Type.ToLower() == "public")
            {
                IsPublic = true;
            }

            DataTable Data = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Data = dbClient.ReadDataTable("SELECT user_id,user_name,hour,minute,message FROM chatlogs WHERE room_id = '" + Room.RoomId + "' ORDER BY timestamp DESC LIMIT 150");
            }

            ServerPacket Message = new ServerPacket(535);
            Message.AppendBoolean(IsPublic);
            Message.AppendUInt(Room.RoomId);
            Message.AppendStringWithBreak(Room.Name);

            if (Data != null)
            {
                Message.AppendInt32(Data.Rows.Count);

                foreach (DataRow Row in Data.Rows)
                {
                    Message.AppendInt32((int)Row["hour"]);
                    Message.AppendInt32((int)Row["minute"]);
                    Message.AppendUInt((uint)Row["user_id"]);
                    Message.AppendStringWithBreak((string)Row["user_name"]);
                    Message.AppendStringWithBreak((string)Row["message"]);
                }
            }
            else
            {
                Message.AppendInt32(0);
            }

            return Message;
        }

        #endregion
    }
}