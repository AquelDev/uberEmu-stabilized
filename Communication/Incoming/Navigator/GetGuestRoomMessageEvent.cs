using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Navigator
{
    class GetGuestRoomMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint RoomId = Packet.PopWiredUInt();
            bool LoadingState = Packet.PopWiredBoolean();
            bool Following = Packet.PopWiredBoolean();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (Data == null)
            {
                return;
            }
            ServerPacket packet = new ServerPacket(454);
            packet.AppendBoolean(LoadingState); 
            Data.Serialize(packet, false);
            packet.AppendBoolean(Following);
            packet.AppendBoolean(LoadingState);
            Session.SendPacket(packet);
        }
    }
}
