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
    class RoomDimmerChangeStateMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.MoodlightData == null)
            {
                return;
            }

            RoomItem Item = null;
            foreach (RoomItem I in Room.Items)
            {
                if (I.GetBaseItem().InteractionType.ToLower() == "dimmer")
                {
                    Item = I;
                    break;
                }
            }

            if (Item == null)
            {
                return;
            }

            if (Room.MoodlightData.Enabled)
            {
                Room.MoodlightData.Disable();
            }
            else
            {
                Room.MoodlightData.Enable();
            }

            Item.ExtraData = Room.MoodlightData.GenerateExtraData();
            Item.UpdateState();
        }
    }
}
