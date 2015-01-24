using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Navigator
{
    class DeleteFavouriteRoomMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint Id = Packet.PopWiredUInt();

            Session.GetHabbo().FavoriteRooms.Remove(Id);

            ServerPacket packet = new ServerPacket(459);
            packet.AppendUInt(Id);
            packet.AppendBoolean(false);
            Session.SendPacket(packet);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM user_favorites WHERE user_id = '" + Session.GetHabbo().Id + "' AND room_id = '" + Id + "' LIMIT 1");
            }
        }
    }
}
