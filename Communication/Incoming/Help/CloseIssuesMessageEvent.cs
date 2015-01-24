using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class CloseIssuesMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Result = Packet.PopWiredInt32(); // result, 1 = useless, 2 = abusive, 3 = resolved
            int Junk = Packet.PopWiredInt32(); // ? 
            uint TicketId = Packet.PopWiredUInt(); // id

            UberEnvironment.GetGame().GetModerationTool().CloseTicket(Session, TicketId, Result);
        }
    }
}
