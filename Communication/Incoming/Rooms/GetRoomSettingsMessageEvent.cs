using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetRoomSettingsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            ServerPacket packet = new ServerPacket(465);
            packet.AppendUInt(Room.RoomId);
            packet.AppendStringWithBreak(Room.Name);
            packet.AppendStringWithBreak(Room.Description);
            packet.AppendInt32(Room.State);
            packet.AppendInt32(Room.Category);
            packet.AppendInt32(Room.UsersMax);
            packet.AppendInt32(25);
            packet.AppendInt32(Room.TagCount);

            foreach (string Tag in Room.Tags)
            {
                packet.AppendStringWithBreak(Tag);
            }

            packet.AppendInt32(Room.UsersWithRights.Count); // users /w rights count

            foreach (uint UserId in Room.UsersWithRights)
            {
                packet.AppendUInt(UserId);
                packet.AppendStringWithBreak(UberEnvironment.GetGame().GetClientManager().GetNameById(UserId));
            }

            packet.AppendInt32(Room.UsersWithRights.Count); // users /w rights count

            packet.AppendBoolean(Room.AllowPets); // allows pets in room - pet system lacking, so always off
            packet.AppendBoolean(Room.AllowPetsEating); // allows pets to eat your food - pet system lacking, so always off
            packet.AppendBoolean(Room.AllowWalkthrough);
            packet.AppendBoolean(Room.Hidewall);

            Session.SendPacket(packet);
        }
    }
}
