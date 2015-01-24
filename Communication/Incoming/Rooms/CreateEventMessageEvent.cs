using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class CreateEventMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.Event != null || Room.State != 0)
            {
                return;
            }

            int _category = Packet.PopWiredInt32();
            string _name = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());
            string _description = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());
            int _count = Packet.PopWiredInt32();

            Room.Event = new RoomEvent(Room.RoomId, _name, _description, _category, null);
            Room.Event.Tags = new List<string>();

            for (int i = 0; i < _count; i++)
            {
                Room.Event.Tags.Add(UberEnvironment.FilterInjectionChars(Packet.PopFixedString()));
            }

            Room.SendMessage(Room.Event.Serialize(Session));
        }
    }
}
