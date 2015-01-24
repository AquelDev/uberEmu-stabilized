using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Users.Messenger;
using Uber.Messages;

namespace Uber.Communication.Incoming.Messenger
{
    class AcceptBuddyMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Amount = Packet.PopWiredInt32();

            for (int i = 0; i < Amount; i++)
            {
                uint RequestId = Packet.PopWiredUInt();

                MessengerRequest MessRequest = Session.GetHabbo().GetMessenger().GetRequest(RequestId);

                if (MessRequest == null)
                {
                    continue;
                }

                if (MessRequest.To != Session.GetHabbo().Id)
                {
                    // not this user's request. filthy haxxor!
                    return;
                }

                if (!Session.GetHabbo().GetMessenger().FriendshipExists(MessRequest.To, MessRequest.From))
                {
                    Session.GetHabbo().GetMessenger().CreateFriendship(MessRequest.From);
                }

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
        }
    }
}
