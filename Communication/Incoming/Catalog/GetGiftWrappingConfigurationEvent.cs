using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class GetGiftWrappingConfigurationEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(620);
            packet.AppendInt32(1);
            packet.AppendInt32(1);
            packet.AppendInt32(10);
            packet.AppendInt32(3064);
            packet.AppendInt32(3065);
            packet.AppendInt32(3066);
            packet.AppendInt32(3067);
            packet.AppendInt32(3068);
            packet.AppendInt32(3069);
            packet.AppendInt32(3070);
            packet.AppendInt32(3071);
            packet.AppendInt32(3072);
            packet.AppendInt32(3073);
            packet.AppendInt32(7);
            packet.AppendInt32(0);
            packet.AppendInt32(1);
            packet.AppendInt32(2);
            packet.AppendInt32(3);
            packet.AppendInt32(4);
            packet.AppendInt32(5);
            packet.AppendInt32(6);
            packet.AppendInt32(11);
            packet.AppendInt32(0);
            packet.AppendInt32(1);
            packet.AppendInt32(2);
            packet.AppendInt32(3);
            packet.AppendInt32(4);
            packet.AppendInt32(5);
            packet.AppendInt32(6);
            packet.AppendInt32(7);
            packet.AppendInt32(8);
            packet.AppendInt32(9);
            packet.AppendInt32(10);
            packet.AppendInt32(1);
            Session.SendPacket(packet);
        }
    }
}
