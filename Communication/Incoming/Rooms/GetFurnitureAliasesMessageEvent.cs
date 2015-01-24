using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetFurnitureAliasesMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().LoadingRoom <= 0)
            {
                return;
            }

            ServerPacket packet = new ServerPacket(297);
            packet.AppendInt32(0);
            Session.SendPacket(packet);
        }
    }
}
