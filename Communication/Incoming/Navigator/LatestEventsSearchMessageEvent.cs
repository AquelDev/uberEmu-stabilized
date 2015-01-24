using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Navigator
{
    class LatestEventsSearchMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            int Category = int.Parse(Packet.PopFixedString());

            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeEventListing(Session, Category));
        }
    }
}
