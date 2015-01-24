using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class GetMarketplaceConfigurationMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(612);
            packet.AppendInt32(1);
            packet.AppendInt32(1);
            packet.AppendInt32(1);
            packet.AppendInt32(5);
            packet.AppendInt32(1);
            packet.AppendInt32(10000);
            packet.AppendInt32(48);
            packet.AppendInt32(7);
            Session.SendPacket(packet);
        }
    }
}
