using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class OpenFlatConnectionMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint Id = Packet.PopWiredUInt();
            string Password = Packet.PopFixedString();
            Packet.PopWiredInt32();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "private")
            {
                return;
            }

            Session.PrepareRoomForUser(Id, Password);
        }
    }
}
