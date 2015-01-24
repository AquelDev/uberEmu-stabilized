using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Rooms
{
    class DeleteRoomMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint RoomId = Packet.PopWiredUInt();
            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (Data == null || Data.Owner.ToLower() != Session.GetHabbo().Username.ToLower())
            {
                return;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM rooms WHERE id = '" + Data.Id + "' LIMIT 1");
                dbClient.ExecuteQuery("DELETE FROM user_favorites WHERE room_id = '" + Data.Id + "'");
                dbClient.ExecuteQuery("DELETE FROM room_items WHERE room_id = '" + Data.Id + "'");
                dbClient.ExecuteQuery("DELETE FROM room_rights WHERE room_id = '" + Data.Id + "'");
                dbClient.ExecuteQuery("UPDATE users SET home_room = '0' WHERE home_room = '" + Data.Id + "'");
                // todo: delete room stuff
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Data.Id);

            if (Room != null)
            {
                foreach (RoomUser User in Room.UserList)
                {
                    if (User.IsBot)
                    {
                        continue;
                    }

                    User.GetClient().SendPacket(new ServerPacket(18));
                    User.GetClient().GetHabbo().OnLeaveRoom();
                }

                UberEnvironment.GetGame().GetRoomManager().UnloadRoom(Data.Id);
            }

            ServerPacket packet = new ServerPacket(101);
            Session.SendPacket(packet);

            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, -3));
        }
    }
}
