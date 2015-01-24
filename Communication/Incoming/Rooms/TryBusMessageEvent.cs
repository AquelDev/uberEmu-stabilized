using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class TryBusMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(81);
            packet.AppendStringWithBreak("The Uber Infobus is not yet in use.");
            Session.SendPacket(packet);
        }
    }
}
