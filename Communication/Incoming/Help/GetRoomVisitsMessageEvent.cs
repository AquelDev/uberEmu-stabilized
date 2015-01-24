using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetRoomVisitsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint UserId = Packet.PopWiredUInt();

            Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeRoomVisits(UserId));
        }
    }
}
