using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetPublicSpaceCastLibsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint Id = Packet.PopWiredUInt();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "public")
            {
                return;
            }

            ServerPacket packet = new ServerPacket(453);
            packet.AppendUInt(Data.Id);
            packet.AppendStringWithBreak(Data.CCTs);
            packet.AppendUInt(Data.Id);
            Session.SendPacket(packet);
        }
    }
}
