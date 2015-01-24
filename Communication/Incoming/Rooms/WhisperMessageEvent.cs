using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Rooms
{
    class WhisperMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            if (Session.GetHabbo().Muted)
            {
                Session.SendNotif("You are muted.");
                return;
            }

            string Params = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());
            string ToUser = Params.Split(' ')[0];
            string Message = Params.Substring(ToUser.Length + 1);

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser User2 = Room.GetRoomUserByHabbo(ToUser);

            ServerPacket TellMsg = new ServerPacket();
            TellMsg.Init(25);
            TellMsg.AppendInt32(User.VirtualId);
            TellMsg.AppendStringWithBreak(Message);
            TellMsg.AppendBoolean(false);

            if (User != null && !User.IsBot)
            {
                User.GetClient().SendPacket(TellMsg);
            }

            User.Unidle();

            if (User2 != null && !User2.IsBot)
            {
                if (!User2.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
                {
                    User2.GetClient().SendPacket(TellMsg);
                }

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("message", "<Whisper to " + User2.GetClient().GetHabbo().Username + ">: " + Message);
                    dbClient.ExecuteQuery("INSERT INTO chatlogs (user_id,room_id,hour,minute,timestamp,message,user_name,full_date) VALUES ('" + Session.GetHabbo().Id + "','" + Room.RoomId + "','" + DateTime.Now.Hour + "','" + DateTime.Now.Minute + "','" + UberEnvironment.GetUnixTimestamp() + "',@message,'" + Session.GetHabbo().Username + "','" + DateTime.Now.ToLongDateString() + "')");
                }
            }
        }
    }
}
