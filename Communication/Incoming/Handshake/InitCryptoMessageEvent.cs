using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.Communication.Outgoing;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Handshake
{
    class InitCryptoMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(ServerPacketHeader.SessionParamsMessageComposer);
            packet.AppendInt32(0);
            Session.SendPacket(packet);
        }
    }
}
