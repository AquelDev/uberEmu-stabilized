using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class CanCreateFlatMessageEventMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(512);
            packet.AppendBoolean(false); // true = show error with number below
            packet.AppendInt32(99999);
            Session.SendPacket(packet);

            // todo: room limit
        }
    }
}
