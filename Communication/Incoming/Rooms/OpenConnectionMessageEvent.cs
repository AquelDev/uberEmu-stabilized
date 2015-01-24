using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class OpenConnectionMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Packet.PopWiredInt32();
            uint Id = Packet.PopWiredUInt();
            Packet.PopWiredInt32();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "public")
            {
                return;
            }

            Session.PrepareRoomForUser(Data.Id, "");
        }
    }
}
