using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class PickIssuesMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Junk = Packet.PopWiredInt32();
            uint TicketId = Packet.PopWiredUInt();
            UberEnvironment.GetGame().GetModerationTool().PickTicket(Session, TicketId);
        }
    }
}
