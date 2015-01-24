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
    class PickupObjectMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Packet.PopWiredInt32();

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            RoomItem Item = Room.GetItem(Packet.PopWiredUInt());

            if (Item == null)
            {
                return;
            }

            switch (Item.GetBaseItem().InteractionType.ToLower())
            {
                case "postit":

                    return; // not allowed to pick up post.its
            }

            Room.RemoveFurniture(Session, Item.Id);
            Session.GetHabbo().GetInventoryComponent().AddItem(Item.Id, Item.BaseItem, Item.ExtraData);
            Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
        }
    }
}
