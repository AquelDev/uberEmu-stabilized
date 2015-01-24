using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class LetUserInMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            string Name = Packet.PopFixedString();
            byte[] Result = Packet.ReadBytes(1);

            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Name);

            if (Client == null)
            {
                return;
            }

            if (Result[0] == Convert.ToByte(65))
            {
                Client.GetHabbo().LoadingChecksPassed = true;

                Session.SendPacket(new ServerPacket(41));
            }
            else
            {
                Session.SendPacket(new ServerPacket(131));
            }
        }
    }
}
