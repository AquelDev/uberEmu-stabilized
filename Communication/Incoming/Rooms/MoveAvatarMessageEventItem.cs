using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Items;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class MoveAvatarMessageEventItem : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            RoomItem Item = Room.GetItem(Packet.PopWiredUInt());

            if (Item == null)
            {
                return;
            }

            int x = Packet.PopWiredInt32();
            int y = Packet.PopWiredInt32();
            int Rotation = Packet.PopWiredInt32();
            Packet.PopWiredInt32();

            Room.SetFloorItem(Session, Item, x, y, Rotation, false);
        }
    }
}
