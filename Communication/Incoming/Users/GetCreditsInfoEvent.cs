using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Users
{
    class GetCreditsInfoEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().UpdateCreditsBalance(false);
            Session.GetHabbo().UpdateActivityPointsBalance(false);
        }
    }
}
