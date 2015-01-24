using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Navigator
{
    class RoomsWithHighestScoreSearchMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, -2));
        }
    }
}
