using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class CallForHelpMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Boolean errorOccured = false;

            if (UberEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                errorOccured = true;
            }

            if (!errorOccured)
            {
                string Message = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());

                int Junk = Packet.PopWiredInt32();
                int Type = Packet.PopWiredInt32();
                uint ReportedUser = Packet.PopWiredUInt();

                UberEnvironment.GetGame().GetModerationTool().SendNewTicket(Session, Type, ReportedUser, Message);
            }

            ServerPacket packet = new ServerPacket(321);
            packet.AppendBoolean(errorOccured);
            Session.SendPacket(packet);
        }
    }
}
