using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetHabboGroupBadgesMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(309);
            packet.AppendStringWithBreak("IcIrDs43103s19014d5a1dc291574a508bc80a64663e61a00");
            Session.SendPacket(packet);
        }
    }
}
