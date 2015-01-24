using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetRoomEntryDataMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().LoadingRoom <= 0)
            {
                return;
            }

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Session.GetHabbo().LoadingRoom);

            if (Data == null)
            {
                return;
            }

            if (Data.Model == null)
            {
                Session.SendNotif("Sorry, model data is missing from this room and therefore cannot be loaded.");
                Session.SendPacket(new ServerPacket(18));
                Session.ClearRoomLoading();
                return;
            }

            Session.SendPacket(Data.Model.SerializeHeightmap());
            Session.SendPacket(Data.Model.SerializeRelativeHeightmap());
        }
    }
}
