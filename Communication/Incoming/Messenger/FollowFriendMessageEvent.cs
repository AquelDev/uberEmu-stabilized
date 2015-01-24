using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Messenger
{
    class FollowFriendMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint BuddyId = Packet.PopWiredUInt();

            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(BuddyId);

            if (Client == null || Client.GetHabbo() == null || !Client.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Client.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            // D^HjTX]X
            ServerPacket packet = new ServerPacket(286);
            packet.AppendBoolean(Room.IsPublic);
            packet.AppendUInt(Client.GetHabbo().CurrentRoomId);
            Session.SendPacket(packet);
        }
    }
}
