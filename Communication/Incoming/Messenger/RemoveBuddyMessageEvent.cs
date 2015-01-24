using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Messenger
{
    class RemoveBuddyMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Requests = Packet.PopWiredInt32();

            for (int i = 0; i < Requests; i++)
            {
                Session.GetHabbo().GetMessenger().DestroyFriendship(Packet.PopWiredUInt());
            }
        }
    }
}
