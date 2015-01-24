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
    class PlaceObjectMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            string PlacementData = Packet.PopFixedString();
            string[] DataBits = PlacementData.Split(' ');
            uint ItemId = uint.Parse(DataBits[0]);

            UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);

            if (Item == null)
            {
                return;
            }

            switch (Item.GetBaseItem().InteractionType.ToLower())
            {
                case "dimmer":

                    if (Room.ItemCountByType("dimmer") >= 1)
                    {
                        Session.SendNotif("You can only have one moodlight in a room.");
                        return;
                    }

                    break;
            }

            // Wall Item
            if (DataBits[1].StartsWith(":"))
            {
                string WallPos = Room.WallPositionCheck(":" + PlacementData.Split(':')[1]);

                if (WallPos == null)
                {
                    ServerPacket packet = new ServerPacket(516);
                    packet.AppendInt32(11);
                    Session.SendPacket(packet);

                    return;
                }

                RoomItem RoomItem = new RoomItem(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, 0, 0, 0.0, 0, WallPos);

                if (Room.SetWallItem(Session, RoomItem))
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
                }
            }
            // Floor Item
            else
            {
                int X = int.Parse(DataBits[1]);
                int Y = int.Parse(DataBits[2]);
                int Rot = int.Parse(DataBits[3]);

                RoomItem RoomItem = new RoomItem(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, 0, 0, 0, 0, "");

                if (Room.SetFloorItem(Session, RoomItem, X, Y, Rot, true))
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
                }
            }
        }
    }
}
