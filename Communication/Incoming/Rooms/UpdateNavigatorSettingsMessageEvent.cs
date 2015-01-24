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
    class UpdateNavigatorSettingsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint RoomId = Packet.PopWiredUInt();
            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (RoomId != 0)
            {
                if (Data == null || Data.Owner.ToLower() != Session.GetHabbo().Username.ToLower())
                {
                    return;
                }
            }

            Session.GetHabbo().HomeRoom = RoomId;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE users SET home_room = '" + RoomId + "' WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            ServerPacket packet = new ServerPacket(455);
            packet.AppendUInt(RoomId);
            Session.SendPacket(packet);
        }
    }
}
