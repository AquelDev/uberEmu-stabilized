using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetPetCommandsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint PetId = Packet.PopWiredUInt();
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            RoomUser PetUser = Room.GetPet(PetId);
            ServerPacket packet = new ServerPacket(605);
            packet.AppendUInt(PetId);
            int level = PetUser.PetData.Level;
            packet.AppendInt32(level);
            for (int i = 0; level > i; )
            {
                i++;
                packet.AppendInt32(i - 1);
            }
            Session.SendPacket(packet);
        }
    }
}
