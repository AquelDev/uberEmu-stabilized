using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class CreateFlatMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            string RoomName = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());
            string ModelName = Packet.PopFixedString();
            string RoomState = Packet.PopFixedString(); // unused, room open by default on creation. may be added in later build of Habbo?

            RoomData NewRoom = UberEnvironment.GetGame().GetRoomManager().CreateRoom(Session, RoomName, ModelName);

            if (NewRoom != null)
            {
                ServerPacket packet = new ServerPacket(59);
                packet.AppendUInt(NewRoom.Id);
                packet.AppendStringWithBreak(NewRoom.Name);
                Session.SendPacket(packet);
            }
        }
    }
}
