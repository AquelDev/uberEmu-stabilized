using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetModeratorRoomInfoMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint RoomId = Packet.PopWiredUInt();
            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId);

            Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeRoomTool(Data));
        }
    }
}
