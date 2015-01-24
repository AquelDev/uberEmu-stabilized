using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Uber.HabboHotel.Users.Badges;
using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Misc;
using Uber.Storage;

namespace Uber.Messages
{
    partial class GameClientMessageHandler
    {
        private void InfoRetrieveMessageEvent()
        {

        }

        private void GetCreditsInfoEvent()
        {
            Session.GetHabbo().UpdateCreditsBalance(false);
            Session.GetHabbo().UpdateActivityPointsBalance(false);
        }

        private void ScrGetUserInfoMessageEvent()
        {
            string SubscriptionId = Request.PopFixedString();

            GetResponse().Init(7);
            GetResponse().AppendStringWithBreak(SubscriptionId.ToLower());

            if (Session.GetHabbo().GetSubscriptionManager().HasSubscription(SubscriptionId))
            {
                Double Expire = Session.GetHabbo().GetSubscriptionManager().GetSubscription(SubscriptionId).ExpireTime;
                Double TimeLeft = Expire - UberEnvironment.GetUnixTimestamp();
                int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
                int MonthsLeft = TotalDaysLeft / 31;

                if (MonthsLeft >= 1) MonthsLeft--;

                GetResponse().AppendInt32(TotalDaysLeft - (MonthsLeft * 31));
                GetResponse().AppendBoolean(true);
                GetResponse().AppendInt32(MonthsLeft);
                GetResponse().AppendInt32(1);
                GetResponse().AppendInt32(1);

                if (Session.GetHabbo().HasFuse("fuse_use_vip_outfits"))
                {
                    GetResponse().AppendInt32(2);
                }
                else
                {
                    GetResponse().AppendInt32(1);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    GetResponse().AppendInt32(0);
                }
            }

            SendResponse();
        }

        private void GetBadgesEvent()
        {
            Session.SendPacket(Session.GetHabbo().GetBadgeComponent().Serialize());
        }

        private void SetActivatedBadgesEvent()
        {
            Session.GetHabbo().GetBadgeComponent().ResetSlots();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE user_badges SET badge_slot = '0' WHERE user_id = '" + Session.GetHabbo().Id + "'");
            }

            while (Request.RemainingLength > 0)
            {
                int Slot = Request.PopWiredInt32();
                string Badge = Request.PopFixedString();

                if (Badge.Length == 0)
                {
                    continue;
                }

                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Badge) || Slot < 1 || Slot > 5)
                {
                    // zomg haxx0r
                    return;
                }

                Session.GetHabbo().GetBadgeComponent().GetBadge(Badge).Slot = Slot;

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("slotid", Slot);
                    dbClient.AddParamWithValue("badge", Badge);
                    dbClient.AddParamWithValue("userid", Session.GetHabbo().Id);
                    dbClient.ExecuteQuery("UPDATE user_badges SET badge_slot = @slotid WHERE badge_id = @badge AND user_id = @userid LIMIT 1");
                }
            }

            ServerPacket Message = new ServerPacket(228);
            Message.AppendUInt(Session.GetHabbo().Id);
            Message.AppendInt32(Session.GetHabbo().GetBadgeComponent().EquippedCount);

            foreach (Badge Badge in Session.GetHabbo().GetBadgeComponent().BadgeList)
            {
                if (Badge.Slot <= 0)
                {
                    continue;
                }

                Message.AppendInt32(Badge.Slot);
                Message.AppendStringWithBreak(Badge.Code);
            }

            if (Session.GetHabbo().InRoom && UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId) != null)
            {
                UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).SendMessage(Message);
            }
            else
            {
                Session.SendPacket(Message);
            }
        }

        private void GetAchievementsEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetAchievementManager().SerializeAchievementList(Session));
        }

        private void UpdateFigureDataMessageEvent()
        {
            if (Session.GetHabbo().MutantPenalty)
            {
                Session.SendNotif("Because of a penalty or restriction on your account, you are not allowed to change your look.");
                return;
            }

            string Gender = Request.PopFixedString().ToUpper();
            string Look = UberEnvironment.FilterInjectionChars(Request.PopFixedString());

            if (!AntiMutant.ValidateLook(Look, Gender))
            {
                return;
            }

            Session.GetHabbo().Look = Look;
            Session.GetHabbo().Gender = Gender.ToLower();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("look", Look);
                dbClient.AddParamWithValue("gender", Gender);
                dbClient.ExecuteQuery("UPDATE users SET look = @look, gender = @gender WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            UberEnvironment.GetGame().GetAchievementManager().UnlockAchievement(Session, 1, 1);

            Session.GetMessageHandler().GetResponse().Init(266);
            Session.GetMessageHandler().GetResponse().AppendInt32(-1);
            Session.GetMessageHandler().GetResponse().AppendStringWithBreak(Session.GetHabbo().Look);
            Session.GetMessageHandler().GetResponse().AppendStringWithBreak(Session.GetHabbo().Gender.ToLower());
            Session.GetMessageHandler().GetResponse().AppendStringWithBreak(Session.GetHabbo().Motto);
            Session.GetMessageHandler().SendResponse();

            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;

                if (Room == null)
                {
                    return;
                }

                RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                ServerPacket RoomUpdate = new ServerPacket(266);
                RoomUpdate.AppendInt32(User.VirtualId);
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Look);
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Gender.ToLower());
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Motto);
                Room.SendMessage(RoomUpdate);
            }
        }

        private void GetWardrobeMessageEvent()
        {
            GetResponse().Init(267);
            GetResponse().AppendBoolean(Session.GetHabbo().HasFuse("fuse_use_wardrobe"));

            if (Session.GetHabbo().HasFuse("fuse_use_wardrobe"))
            {
                DataTable WardrobeData = null;

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("userid", Session.GetHabbo().Id);
                    WardrobeData = dbClient.ReadDataTable("SELECT * FROM user_wardrobe WHERE user_id = @userid");
                }

                if (WardrobeData == null)
                {
                    GetResponse().AppendInt32(0);
                }
                else
                {
                    GetResponse().AppendInt32(WardrobeData.Rows.Count);

                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        GetResponse().AppendUInt((uint)Row["slot_id"]);
                        GetResponse().AppendStringWithBreak((string)Row["look"]);
                        GetResponse().AppendStringWithBreak((string)Row["gender"]);
                    }
                }
            }

            SendResponse();
        }
        private void ChangeMottoMessageEvent()
        {
            string Motto = UberEnvironment.FilterInjectionChars(Request.PopFixedString());

            if (Motto == Session.GetHabbo().Motto) // Prevents spam?
            {
                return;
            }

            Session.GetHabbo().Motto = Motto;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("motto", Motto);
                dbClient.ExecuteQuery("UPDATE users SET motto = @motto WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            GetResponse().Init(484);
            Session.GetMessageHandler().GetResponse().AppendInt32(-1);
            Session.GetMessageHandler().GetResponse().AppendStringWithBreak(Session.GetHabbo().Motto);
            Session.GetMessageHandler().SendResponse();
            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;

                if (Room == null)
                {
                    return;
                }

                RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                ServerPacket RoomUpdate = new ServerPacket(266);
                RoomUpdate.AppendInt32(User.VirtualId);
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Look); // Need this to reload User or they will be invisible
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Gender.ToLower()); // Need this to reload User or they will be invisible
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Motto); // Update The User Motto
                Room.SendMessage(RoomUpdate);
            }

        }

        private void SaveWardrobeOutfitMessageEvent()
        {
            uint SlotId = Request.PopWiredUInt();

            string Look = Request.PopFixedString();
            string Gender = Request.PopFixedString();

            if (!AntiMutant.ValidateLook(Look, Gender))
            {
                return;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("userid", Session.GetHabbo().Id);
                dbClient.AddParamWithValue("slotid", SlotId);
                dbClient.AddParamWithValue("look", Look);
                dbClient.AddParamWithValue("gender", Gender.ToUpper());

                if (dbClient.ReadDataRow("SELECT null FROM user_wardrobe WHERE user_id = @userid AND slot_id = @slotid LIMIT 1") != null)
                {
                    dbClient.ExecuteQuery("UPDATE user_wardrobe SET look = @look, gender = @gender WHERE user_id = @userid AND slot_id = @slotid LIMIT 1");
                }
                else
                {
                    dbClient.ExecuteQuery("INSERT INTO user_wardrobe (user_id,slot_id,look,gender) VALUES (@userid,@slotid,@look,@gender)");
                }
            }
        }

        private void GetPetInventoryEvent()
        {
            if (Session.GetHabbo().GetInventoryComponent() == null)
            {
                return;
            }

            Session.SendPacket(Session.GetHabbo().GetInventoryComponent().SerializePetInventory());
        }

        public void RegisterUsers()
        {
            RequestHandlers[7] = new RequestHandler(InfoRetrieveMessageEvent);
            RequestHandlers[8] = new RequestHandler(GetCreditsInfoEvent);
            RequestHandlers[26] = new RequestHandler(ScrGetUserInfoMessageEvent);

            RequestHandlers[157] = new RequestHandler(GetBadgesEvent);
            RequestHandlers[158] = new RequestHandler(SetActivatedBadgesEvent);
            RequestHandlers[370] = new RequestHandler(GetAchievementsEvent);

            RequestHandlers[44] = new RequestHandler(UpdateFigureDataMessageEvent);
            RequestHandlers[375] = new RequestHandler(GetWardrobeMessageEvent);
            RequestHandlers[376] = new RequestHandler(SaveWardrobeOutfitMessageEvent);

            RequestHandlers[404] = new RequestHandler(RequestFurniInventoryEvent);
            RequestHandlers[484] = new RequestHandler(ChangeMottoMessageEvent);
            RequestHandlers[3000] = new RequestHandler(GetPetInventoryEvent);
        }
    }
}
