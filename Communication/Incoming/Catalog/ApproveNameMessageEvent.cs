using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class ApproveNameMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(36);
            packet.AppendInt32(UberEnvironment.GetGame().GetCatalog().CheckPetName(Packet.PopFixedString()) ? 0 : 2);
            Session.SendPacket(packet);
        }
    }
}
