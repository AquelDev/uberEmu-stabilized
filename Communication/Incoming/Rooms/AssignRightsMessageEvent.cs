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
    class AssignRightsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint UserId = Packet.PopWiredUInt();

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            RoomUser RoomUser = Room.GetRoomUserByHabbo(UserId);

            if (Room == null || !Room.CheckRights(Session, true) || RoomUser == null || RoomUser.IsBot)
            {
                return;
            }

            if (Room.UsersWithRights.Contains(UserId))
            {
                // todo: fix silly bug
                Session.SendNotif("User already has rights! (There appears to be a bug with the rights button, we are looking into it - for now rely on 'Advanced settings')");
                return;
            }

            Room.UsersWithRights.Add(UserId);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("INSERT INTO room_rights (room_id,user_id) VALUES ('" + Room.RoomId + "','" + UserId + "')");
            }

            ServerPacket packet = new ServerPacket(510);
            packet.AppendUInt(Room.RoomId);
            packet.AppendUInt(UserId);
            packet.AppendStringWithBreak(RoomUser.GetClient().GetHabbo().Username);
            Session.SendPacket(packet);

            RoomUser.AddStatus("flatcrtl", "");
            RoomUser.UpdateNeeded = true;

            RoomUser.GetClient().SendPacket(new ServerPacket(42));
        }
    }
}
