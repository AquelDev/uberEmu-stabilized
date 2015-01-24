using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetUserChatlogMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeUserChatlog(Packet.PopWiredUInt()));
        }
    }
}
