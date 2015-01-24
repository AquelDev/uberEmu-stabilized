using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Catalogs;
using Uber.Storage;

namespace Uber.Messages
{
    partial class GameClientMessageHandler
    {
        private void AddFavouriteRoomMessageEvent()
        {
            uint Id = Request.PopWiredUInt();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || Session.GetHabbo().FavoriteRooms.Contains(Id) || Data.Type == "public")
            {
                GetResponse().Init(33);
                GetResponse().AppendInt32(-9001);
                SendResponse();

                return;
            }

            GetResponse().Init(459);
            GetResponse().AppendUInt(Id);
            GetResponse().AppendBoolean(true);
            SendResponse();

            Session.GetHabbo().FavoriteRooms.Add(Id);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("INSERT INTO user_favorites (user_id,room_id) VALUES ('" + Session.GetHabbo().Id + "','" + Id + "')");
            }
        }

        private void DeleteFavouriteRoomMessageEvent()
        {
            uint Id = Request.PopWiredUInt();

            Session.GetHabbo().FavoriteRooms.Remove(Id);

            GetResponse().Init(459);
            GetResponse().AppendUInt(Id);
            GetResponse().AppendBoolean(false);
            SendResponse();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM user_favorites WHERE user_id = '" + Session.GetHabbo().Id + "' AND room_id = '" + Id + "' LIMIT 1");
            }
        }

        private void QuitMessageEvent()
        {
            if (Session.GetHabbo().InRoom)
            {
                UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).RemoveUserFromRoom(Session, true, false);
               
            }

        }

        private void GetUserFlatCatsMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeFlatCategories());
        }

        private void GetBuddyRequestsMessageEvent()
        {
            // ???????????????????????????????
        }

        private void GetOfficialRoomsMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializePublicRooms());
        }

        private void GetGuestRoomMessageEvent()
        {
            uint RoomId = Request.PopWiredUInt();
            bool unk = Request.PopWiredBoolean();
            bool unk2 = Request.PopWiredBoolean();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (Data == null)
            {
                return;
            }
            GetResponse().Init(454);
            GetResponse().AppendInt32(0);
            Data.Serialize(GetResponse(), false);
            SendResponse();
        }

        private void PopularRoomsSearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, int.Parse(Request.PopFixedString())));
        }

        private void RoomsWithHighestScoreSearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, -2));
        }

        private void MyFriendsRoomsSearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, -4));
        }

        private void RoomsWhereMyFriendsAreSearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, -5));
        }

        private void MyRoomsSearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, -3));
        }

        private void MyFavouriteRoomsSearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeFavoriteRooms(Session));
        }

        private void MyRoomHistorySearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRecentRooms(Session));
        }

        private void LatestEventsSearchMessageEvent()
        {
            int Category = int.Parse(Request.PopFixedString());

            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeEventListing(Session, Category));
        }

        private void GetPopularRoomTagsMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializePopularRoomTags());
        }

        private void RoomTextSearchMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeSearchResults(Request.PopFixedString()));
        }

        private void RoomTagSearchMessageEvent()
        {
            int junk = Request.PopWiredInt32();
            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeSearchResults(Request.PopFixedString()));
        }

        public void RegisterNavigator()
        {
            RequestHandlers[19] = new RequestHandler(AddFavouriteRoomMessageEvent);
            RequestHandlers[20] = new RequestHandler(DeleteFavouriteRoomMessageEvent);
            RequestHandlers[53] = new RequestHandler(QuitMessageEvent);
            RequestHandlers[151] = new RequestHandler(GetUserFlatCatsMessageEvent);
            RequestHandlers[233] = new RequestHandler(GetBuddyRequestsMessageEvent);
            RequestHandlers[380] = new RequestHandler(GetOfficialRoomsMessageEvent);
            RequestHandlers[385] = new RequestHandler(GetGuestRoomMessageEvent);
            RequestHandlers[430] = new RequestHandler(PopularRoomsSearchMessageEvent);
            RequestHandlers[431] = new RequestHandler(RoomsWithHighestScoreSearchMessageEvent);
            RequestHandlers[432] = new RequestHandler(MyFriendsRoomsSearchMessageEvent);
            RequestHandlers[433] = new RequestHandler(RoomsWhereMyFriendsAreSearchMessageEvent);
            RequestHandlers[434] = new RequestHandler(MyRoomsSearchMessageEvent);
            RequestHandlers[435] = new RequestHandler(MyFavouriteRoomsSearchMessageEvent);
            RequestHandlers[436] = new RequestHandler(MyRoomHistorySearchMessageEvent);
            RequestHandlers[439] = new RequestHandler(LatestEventsSearchMessageEvent);
            RequestHandlers[382] = new RequestHandler(GetPopularRoomTagsMessageEvent);
            RequestHandlers[437] = new RequestHandler(RoomTextSearchMessageEvent);
            RequestHandlers[438] = new RequestHandler(RoomTagSearchMessageEvent);

        }
    }
}
