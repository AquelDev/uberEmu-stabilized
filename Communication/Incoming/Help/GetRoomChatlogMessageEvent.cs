using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetRoomChatlogMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            int Junk = Packet.PopWiredInt32();
            uint RoomId = Packet.PopWiredUInt();

            if (UberEnvironment.GetGame().GetRoomManager().GetRoom(RoomId) != null)
            {
                Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeRoomChatlog(RoomId));
            }
        }
    }
}
