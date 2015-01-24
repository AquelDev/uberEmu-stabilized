using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Messenger
{
    class SendRoomInviteMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            int count = Packet.PopWiredInt32();

            List<uint> UserIds = new List<uint>();

            for (int i = 0; i < count; i++)
            {
                UserIds.Add(Packet.PopWiredUInt());
            }

            string message = UberEnvironment.FilterInjectionChars(Packet.PopFixedString(), true);

            ServerPacket Message = new ServerPacket(135);
            Message.AppendUInt(Session.GetHabbo().Id);
            Message.AppendStringWithBreak(message);

            foreach (uint Id in UserIds)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id, Id))
                {
                    continue;
                }

                GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Id);

                if (Client == null)
                {
                    return;
                }

                Client.SendPacket(Message);
            }
        }
    }
}
