using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Navigator
{
    class AddFavouriteRoomMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint Id = Packet.PopWiredUInt();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || Session.GetHabbo().FavoriteRooms.Contains(Id) || Data.Type == "public")
            {
                ServerPacket packet = new ServerPacket(33);
                packet.AppendInt32(-9001);
                Session.SendPacket(packet);

                return;
            }

            ServerPacket _packet = new ServerPacket(459);
            _packet.AppendUInt(Id);
            _packet.AppendBoolean(true);
            Session.SendPacket(_packet);

            Session.GetHabbo().FavoriteRooms.Add(Id);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("INSERT INTO user_favorites (user_id,room_id) VALUES ('" + Session.GetHabbo().Id + "','" + Id + "')");
            }
        }
    }
}
