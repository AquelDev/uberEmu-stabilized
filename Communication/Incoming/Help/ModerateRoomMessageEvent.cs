using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class ModerateRoomMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint RoomId = Packet.PopWiredUInt();
            Boolean ActOne = Packet.PopWiredBoolean(); // set room lock to doorbell
            Boolean ActTwo = Packet.PopWiredBoolean(); // set room to inappropiate
            Boolean ActThree = Packet.PopWiredBoolean(); // kick all users

            UberEnvironment.GetGame().GetModerationTool().PerformRoomAction(Session, RoomId, ActThree, ActOne, ActTwo);
        }
    }
}
