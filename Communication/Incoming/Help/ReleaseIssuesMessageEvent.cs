using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class ReleaseIssuesMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int amount = Packet.PopWiredInt32();

            for (int i = 0; i < amount; i++)
            {
                uint TicketId = Packet.PopWiredUInt();

                UberEnvironment.GetGame().GetModerationTool().ReleaseTicket(Session, TicketId);
            }
        }
    }
}
