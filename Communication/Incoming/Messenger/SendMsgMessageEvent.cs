using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Messenger
{
    class SendMsgMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint userId = Packet.PopWiredUInt();
            string message = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());

            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);
        }
    }
}
