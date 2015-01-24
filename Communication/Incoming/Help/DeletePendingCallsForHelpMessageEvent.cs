using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class DeletePendingCallsForHelpMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!UberEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                return;
            }

            UberEnvironment.GetGame().GetModerationTool().DeletePendingTicketForUser(Session.GetHabbo().Id);

            ServerPacket packet = new ServerPacket(320);
            Session.SendPacket(packet);
        }
    }
}
