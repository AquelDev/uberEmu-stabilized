using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class UnignoreUserMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            uint Id = Packet.PopWiredUInt();

            if (!Session.GetHabbo().MutedUsers.Contains(Id))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Remove(Id);

            ServerPacket packet = new ServerPacket(419);
            packet.AppendInt32(3);
            Session.SendPacket(packet);
        }
    }
}
