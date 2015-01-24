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
    class RoomDimmerGetPresetsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.MoodlightData == null)
            {
                return;
            }

            ServerPacket packet = new ServerPacket(365);
            packet.AppendInt32(Room.MoodlightData.Presets.Count);
            packet.AppendInt32(Room.MoodlightData.CurrentPreset);

            int i = 0;

            foreach (MoodlightPreset Preset in Room.MoodlightData.Presets)
            {
                i++;

                packet.AppendInt32(i);
                packet.AppendInt32(int.Parse(UberEnvironment.BoolToEnum(Preset.BackgroundOnly)) + 1);
                packet.AppendStringWithBreak(Preset.ColorCode);
                packet.AppendInt32(Preset.ColorIntensity);
            }

            Session.SendPacket(packet);
        }
    }
}
