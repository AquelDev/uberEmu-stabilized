using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Users.Badges;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetSelectedBadgesMessageEvent : IPacketEvent
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
            ServerPacket packet = new ServerPacket(228);
            packet.AppendUInt(User.GetClient().GetHabbo().Id);
            packet.AppendInt32(User.GetClient().GetHabbo().GetBadgeComponent().EquippedCount);

            foreach (Badge Badge in User.GetClient().GetHabbo().GetBadgeComponent().BadgeList)
            {
                if (Badge.Slot <= 0)
                {
                    continue;
                }

                packet.AppendInt32(Badge.Slot);
                packet.AppendStringWithBreak(Badge.Code);
            }

            Session.SendPacket(packet);
        }
    }
}
