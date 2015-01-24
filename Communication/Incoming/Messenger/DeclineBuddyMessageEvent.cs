using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Messenger
{
    class DeclineBuddyMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            // Remove all = @f I H
            // Remove specific = @f H I <reqid>

            int Mode = Packet.PopWiredInt32();
            int Amount = Packet.PopWiredInt32();

            if (Mode == 0 && Amount == 1)
            {
                uint RequestId = Packet.PopWiredUInt();

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
            else if (Mode == 1)
            {
                Session.GetHabbo().GetMessenger().HandleAllRequests();
            }
            else { } // todo: remove breakpoint - eventually -, but leave for a while to make sure the structure is correct and this never happens
        }
    }
}
