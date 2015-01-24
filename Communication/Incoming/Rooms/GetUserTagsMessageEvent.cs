using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetUserTagsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Packet.PopWiredUInt());

            if (User == null || User.IsBot)
            {
                return;
            }

            ServerPacket packet = new ServerPacket(350);
            packet.AppendUInt(User.GetClient().GetHabbo().Id);
            packet.AppendInt32(User.GetClient().GetHabbo().Tags.Count);

            foreach (string Tag in User.GetClient().GetHabbo().Tags)
            {
                packet.AppendStringWithBreak(Tag);
            }

            Session.SendPacket(packet);
        }
    }
}
