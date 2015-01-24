using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Users
{
    class ChangeMottoMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            string Motto = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());

            if (Motto == Session.GetHabbo().Motto) // Prevents spam?
            {
                return;
            }

            Session.GetHabbo().Motto = Motto;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("motto", Motto);
                dbClient.ExecuteQuery("UPDATE users SET motto = @motto WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            ServerPacket packet = new ServerPacket(484);
            packet.AppendInt32(-1);
            packet.AppendStringWithBreak(Session.GetHabbo().Motto);
            Session.SendPacket(packet);
            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;

                if (Room == null)
                {
                    return;
                }

                RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                ServerPacket RoomUpdate = new ServerPacket(266);
                RoomUpdate.AppendInt32(User.VirtualId);
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Look); // Need this to reload User or they will be invisible
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Gender.ToLower()); // Need this to reload User or they will be invisible
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Motto); // Update The User Motto
                Room.SendMessage(RoomUpdate);
            }
        }
    }
}