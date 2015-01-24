﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Uber.HabboHotel.Pets;
using Uber.HabboHotel.Navigators;
using Uber.HabboHotel.Items;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Users.Badges;
using Uber.HabboHotel.Misc;
using Uber.HabboHotel.Pathfinding;
using Uber.HabboHotel.Advertisements;
using Uber.HabboHotel.Users.Messenger;
using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Catalogs;
using Uber.Storage;

namespace Uber.Messages
{
    partial class GameClientMessageHandler
    {
        private void GetInterstitialMessageEvent()
        {
            RoomAdvertisement Ad = UberEnvironment.GetGame().GetAdvertisementManager().GetRandomRoomAdvertisement();

            GetResponse().Init(258);

            if (Ad == null)
            {
                GetResponse().AppendStringWithBreak("");
                GetResponse().AppendStringWithBreak("");
            }
            else
            {
                GetResponse().AppendStringWithBreak(Ad.AdImage);
                GetResponse().AppendStringWithBreak(Ad.AdLink);

                Ad.OnView();
            }

            SendResponse();
        }

        private void GetPublicSpaceCastLibsMessageEvent()
        {
            uint Id = Request.PopWiredUInt();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "public")
            {
                return;
            }

            GetResponse().Init(453);
            GetResponse().AppendUInt(Data.Id);
            GetResponse().AppendStringWithBreak(Data.CCTs);
            GetResponse().AppendUInt(Data.Id);
            SendResponse();
        }

        private void OpenFlatConnectionMessageEvent()
        {
            uint Id = Request.PopWiredUInt();
            string Password = Request.PopFixedString();
            int Junk = Request.PopWiredInt32();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "private")
            {
                return;
            }

            PrepareRoomForUser(Id, Password);
        }

        private void OpenConnectionMessageEvent()
        {
            int Junk = Request.PopWiredInt32();
            uint Id = Request.PopWiredUInt();
            int Junk2 = Request.PopWiredInt32();

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id);

            if (Data == null || Data.Type != "public")
            {
                return;
            }

            PrepareRoomForUser(Data.Id, "");
        }

        private void GetHabboGroupBadgesMessageEvent()
        {
            GetResponse().Init(309);
            GetResponse().AppendStringWithBreak("IcIrDs43103s19014d5a1dc291574a508bc80a64663e61a00");
            SendResponse();
        }

        private void RequestFurniInventoryEvent()
        {
            Session.SendPacket(Session.GetHabbo().GetInventoryComponent().SerializeItemInventory());
        }

        private void GetFurnitureAliasesMessageEvent()
        {
            if (Session.GetHabbo().LoadingRoom <= 0)
            {
                return;
            }

            GetResponse().Init(297);
            GetResponse().AppendInt32(0);
            SendResponse();
        }

        private void GetRoomEntryDataMessageEvent()
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
                ClearRoomLoading();
                return;
            }

            Session.SendPacket(Data.Model.SerializeHeightmap());
            Session.SendPacket(Data.Model.SerializeRelativeHeightmap());
        }

        private void GetRoomAdMessageEvent()
        {
            if (Session.GetHabbo().LoadingRoom <= 0 || !Session.GetHabbo().LoadingChecksPassed)
            {
                return;
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().LoadingRoom);

            if (Room == null)
            {
                return;
            }

            ClearRoomLoading();

            GetResponse().Init(30);

            if (Room.Model.StaticFurniMap != "")
            {
                GetResponse().AppendStringWithBreak(Room.Model.StaticFurniMap);
            }
            else
            {
                GetResponse().AppendInt32(0);
            }

            SendResponse();

            if (Room.Type == "private")
            {
                List<RoomItem> FloorItems = Room.FloorItems;
                List<RoomItem> WallItems = Room.WallItems;

                GetResponse().Init(32);
                GetResponse().AppendInt32(FloorItems.Count);

                foreach (RoomItem Item in FloorItems)
                {
                    Item.Serialize(GetResponse());
                }

                SendResponse();

                GetResponse().Init(45);
                GetResponse().AppendInt32(WallItems.Count);

                foreach (RoomItem Item in WallItems)
                {
                    Item.Serialize(GetResponse());
                }

                SendResponse();
            }

            Room.AddUserToRoom(Session, Session.GetHabbo().SpectatorMode);

            List<RoomUser> UsersToDisplay = new List<RoomUser>();

            foreach (RoomUser User in Room.UserList)
            {
                if (User.IsSpectator)
                {
                    continue;
                }

                UsersToDisplay.Add(User);
            }

            GetResponse().Init(28);
            GetResponse().AppendInt32(UsersToDisplay.Count);

            foreach (RoomUser User in UsersToDisplay)
            {
                User.Serialize(GetResponse());
            }

            SendResponse();

            //GXI
            GetResponse().Init(472);
            GetResponse().AppendBoolean(Room.Hidewall);
            SendResponse();

            if (Room.Type == "public")
            {
                GetResponse().Init(471);
                GetResponse().AppendBoolean(false);
                GetResponse().AppendStringWithBreak(Room.ModelName);
                GetResponse().AppendBoolean(false);
                SendResponse();
            }
            else if (Room.Type == "private")
            {
                GetResponse().Init(471);
                GetResponse().AppendBoolean(true);
                GetResponse().AppendUInt(Room.RoomId);

                if (Room.CheckRights(Session, true))
                {
                    GetResponse().AppendBoolean(true);
                }
                else
                {
                    GetResponse().AppendBoolean(false);
                }

                SendResponse();

                // GQhntX]uberEmu PacketloggingDescriptionHQMSCQFJtag1tag2Ika^SMqurbIHH

                GetResponse().Init(454);
                GetResponse().AppendInt32(1);
                GetResponse().AppendUInt(Room.RoomId);
                GetResponse().AppendInt32(0);
                GetResponse().AppendStringWithBreak(Room.Name);
                GetResponse().AppendStringWithBreak(Room.Owner);
                GetResponse().AppendInt32(Room.State);
                GetResponse().AppendInt32(0);
                GetResponse().AppendInt32(25);
                GetResponse().AppendStringWithBreak(Room.Description);
                GetResponse().AppendInt32(0);
                GetResponse().AppendInt32(1);
                GetResponse().AppendInt32(8228);
                GetResponse().AppendInt32(Room.Category);
                GetResponse().AppendStringWithBreak("");
                GetResponse().AppendInt32(Room.TagCount);

                foreach (string Tag in Room.Tags)
                {
                    GetResponse().AppendStringWithBreak(Tag);
                }

                Room.Icon.Serialize(GetResponse());
                GetResponse().AppendBoolean(false);
                SendResponse();
            }

            ServerPacket Updates = Room.SerializeStatusUpdates(true);

            if (Updates != null)
            {
                Session.SendPacket(Updates);
            }

            foreach (RoomUser User in Room.UserList)
            {
                if (User.IsSpectator)
                {
                    continue;
                }

                if (User.IsDancing)
                {
                    GetResponse().Init(480);
                    GetResponse().AppendInt32(User.VirtualId);
                    GetResponse().AppendInt32(User.DanceId);
                    SendResponse();
                }

                if (User.IsAsleep)
                {
                    GetResponse().Init(486);
                    GetResponse().AppendInt32(User.VirtualId);
                    GetResponse().AppendBoolean(true);
                    SendResponse();
                }

                if (User.CarryItemID > 0 && User.CarryTimer > 0)
                {
                    GetResponse().Init(482);
                    GetResponse().AppendInt32(User.VirtualId);
                    GetResponse().AppendInt32(User.CarryTimer);
                    SendResponse();
                }

                if (!User.IsBot)
                {
                    if (User.GetClient().GetHabbo() != null && User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent() != null && User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().CurrentEffect >= 1)
                    {
                        GetResponse().Init(485);
                        GetResponse().AppendInt32(User.VirtualId);
                        GetResponse().AppendInt32(User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().CurrentEffect);
                        SendResponse();
                    }
                }
            }
        }

        public void PrepareRoomForUser(uint Id, string Password)
        {
            ClearRoomLoading();

            if (UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(Id) == null)
            {
                return;
            }

            if (Session.GetHabbo().InRoom)
            {
                Room OldRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

                if (OldRoom != null)
                {
                    OldRoom.RemoveUserFromRoom(Session, false, false);
                }
            }

            if (!UberEnvironment.GetGame().GetRoomManager().IsRoomLoaded(Id))
            {
                UberEnvironment.GetGame().GetRoomManager().LoadRoom(Id);
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Id);

            if (Room == null)
            {
                return;
            }

            Session.GetHabbo().LoadingRoom = Id;

            if (Room.UserIsBanned(Session.GetHabbo().Id))
            {
                if (Room.HasBanExpired(Session.GetHabbo().Id))
                {
                    Room.RemoveBan(Session.GetHabbo().Id);
                }
                else
                {
                    GetResponse().Init(224);
                    GetResponse().AppendInt32(4);
                    SendResponse();

                    GetResponse().Init(18);
                    SendResponse();

                    return;
                }
            }

            if (Room.UsersNow >= Room.UsersMax)
            {
                if (!UberEnvironment.GetGame().GetRoleManager().RankHasRight(Session.GetHabbo().Rank, "fuse_enter_full_rooms"))
                {
                    GetResponse().Init(224);
                    GetResponse().AppendInt32(1);
                    SendResponse();

                    GetResponse().Init(18);
                    SendResponse();

                    return;
                }
            }

            if (Room.Type == "public")
            {
                if (Room.State > 0 && !Session.GetHabbo().HasFuse("fuse_mod"))
                {
                    Session.SendNotif("This public room is accessible to staff only.");

                    GetResponse().Init(18);
                    SendResponse();

                    return;
                }

                GetResponse().Init(166);
                GetResponse().AppendStringWithBreak("/client/public/" + Room.ModelName + "/" + Room.RoomId);
                SendResponse();
            }
            else if (Room.Type == "private")
            {
                GetResponse().Init(19);
                SendResponse();

                if (!Session.GetHabbo().HasFuse("fuse_enter_any_room") && !Room.CheckRights(Session, true) && !Session.GetHabbo().IsTeleporting)
                {
                    if (Room.State == 1)
                    {
                        if (Room.UserCount == 0)
                        {
                            GetResponse().Init(131);
                            SendResponse();
                        }
                        else
                        {
                            GetResponse().Init(91);
                            GetResponse().AppendStringWithBreak("");
                            SendResponse();

                            ServerPacket RingMessage = new ServerPacket(91);
                            RingMessage.AppendStringWithBreak(Session.GetHabbo().Username);
                            Room.SendMessageToUsersWithRights(RingMessage);
                        }

                        return;
                    }
                    else if (Room.State == 2)
                    {
                        if (Password.ToLower() != Room.Password.ToLower())
                        {
                            GetResponse().Init(33);
                            GetResponse().AppendInt32(-100002);
                            SendResponse();

                            GetResponse().Init(18);
                            SendResponse();

                            return;
                        }
                    }
                }

                GetResponse().Init(166);
                GetResponse().AppendStringWithBreak("/client/private/" + Room.RoomId + "/id");
                SendResponse();
            }

            Session.GetHabbo().LoadingChecksPassed = true;

            LoadRoomForUser();
        }

        private void GoToFlatMessageEvent()
        {
            LoadRoomForUser();
        }

        public void LoadRoomForUser()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().LoadingRoom);

            if (Room == null || !Session.GetHabbo().LoadingChecksPassed)
            {
                return;
            }

            // todo: Room.SerializeGroupBadges()
            GetResponse().Init(309);
            GetResponse().AppendStringWithBreak("IcIrDs43103s19014d5a1dc291574a508bc80a64663e61a00");
            SendResponse();

            GetResponse().Init(69);
            GetResponse().AppendStringWithBreak(Room.ModelName);
            GetResponse().AppendUInt(Room.RoomId);
            SendResponse();

            if (Session.GetHabbo().SpectatorMode)
            {
                GetResponse().Init(254);
                SendResponse();
            }

            if (Room.Type == "private")
            {
                if (Room.Wallpaper != "0.0")
                {
                    GetResponse().Init(46);
                    GetResponse().AppendStringWithBreak("wallpaper");
                    GetResponse().AppendStringWithBreak(Room.Wallpaper);
                    SendResponse();
                }

                if (Room.Floor != "0.0")
                {
                    GetResponse().Init(46);
                    GetResponse().AppendStringWithBreak("floor");
                    GetResponse().AppendStringWithBreak(Room.Floor);
                    SendResponse();
                }

                GetResponse().Init(46);
                GetResponse().AppendStringWithBreak("landscape");
                GetResponse().AppendStringWithBreak(Room.Landscape);
                SendResponse();

                if (Room.CheckRights(Session, true))
                {
                    GetResponse().Init(42);
                    SendResponse();

                    GetResponse().Init(47);
                    SendResponse();
                }
                else if (Room.CheckRights(Session))
                {
                    GetResponse().Init(42);
                    SendResponse();
                }

                GetResponse().Init(345);

                if (Session.GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.CheckRights(Session, true))
                {
                    GetResponse().AppendInt32(Room.Score);
                }
                else
                {
                    GetResponse().AppendInt32(-1);
                }

                SendResponse();

                if (Room.HasOngoingEvent)
                {
                    Session.SendPacket(Room.Event.Serialize(Session));
                }
                else
                {
                    GetResponse().Init(370);
                    GetResponse().AppendStringWithBreak("-1");
                    SendResponse();
                }
            }
        }

        public void ClearRoomLoading()
        {
            Session.GetHabbo().LoadingRoom = 0;
            Session.GetHabbo().LoadingChecksPassed = false;
        }

        private void ChatMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            User.Chat(Session, UberEnvironment.FilterInjectionChars(Request.PopFixedString()), false);
        }

        private void ShoutMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            User.Chat(Session, UberEnvironment.FilterInjectionChars(Request.PopFixedString()), true);
        }

        private void WhisperMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            if (Session.GetHabbo().Muted)
            {
                Session.SendNotif("You are muted.");
                return;
            }

            string Params = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            string ToUser = Params.Split(' ')[0];
            string Message = Params.Substring(ToUser.Length + 1);

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser User2 = Room.GetRoomUserByHabbo(ToUser);

            ServerPacket TellMsg = new ServerPacket();
            TellMsg.Init(25);
            TellMsg.AppendInt32(User.VirtualId);
            TellMsg.AppendStringWithBreak(Message);
            TellMsg.AppendBoolean(false);

            if (User != null && !User.IsBot)
            {
                User.GetClient().SendPacket(TellMsg);
            }

            User.Unidle();

            if (User2 != null && !User2.IsBot)
            {
                if (!User2.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
                {
                    User2.GetClient().SendPacket(TellMsg);
                }

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("message", "<WhisperMessageEvent to " + User2.GetClient().GetHabbo().Username + ">: " + Message);
                    dbClient.ExecuteQuery("INSERT INTO chatlogs (user_id,room_id,hour,minute,timestamp,message,user_name,full_date) VALUES ('" + Session.GetHabbo().Id + "','" + Room.RoomId + "','" + DateTime.Now.Hour + "','" + DateTime.Now.Minute + "','" + UberEnvironment.GetUnixTimestamp() + "',@message,'" + Session.GetHabbo().Username + "','" + DateTime.Now.ToLongDateString() + "')");
                }
            }
        }

        private void MoveAvatarMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null || !User.CanWalk)
            {
                return;
            }

            int MoveAvatarMessageEventX = Request.PopWiredInt32();
            int MoveAvatarMessageEventY = Request.PopWiredInt32();

            if (MoveAvatarMessageEventX == User.X && MoveAvatarMessageEventY == User.Y)
            {
                return;
            }

            User.MoveTo(MoveAvatarMessageEventX, MoveAvatarMessageEventY);
        }

        private void CanCreateFlatMessageEventMessageEvent()
        {
            GetResponse().Init(512);
            GetResponse().AppendBoolean(false); // true = show error with number below
            GetResponse().AppendInt32(99999);
            SendResponse();

            // todo: room limit
        }

        private void CreateFlatMessageEvent()
        {
            string RoomName = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            string ModelName = Request.PopFixedString();
            string RoomState = Request.PopFixedString(); // unused, room open by default on creation. may be added in later build of Habbo?

            RoomData NewRoom = UberEnvironment.GetGame().GetRoomManager().CreateRoom(Session, RoomName, ModelName);

            if (NewRoom != null)
            {
                GetResponse().Init(59);
                GetResponse().AppendUInt(NewRoom.Id);
                GetResponse().AppendStringWithBreak(NewRoom.Name);
                SendResponse();
            }
        }

        private void GetRoomSettingsMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            GetResponse().Init(465);
            GetResponse().AppendUInt(Room.RoomId);
            GetResponse().AppendStringWithBreak(Room.Name);
            GetResponse().AppendStringWithBreak(Room.Description);
            GetResponse().AppendInt32(Room.State);
            GetResponse().AppendInt32(Room.Category);
            GetResponse().AppendInt32(Room.UsersMax);
            GetResponse().AppendInt32(25);
            GetResponse().AppendInt32(Room.TagCount);

            foreach (string Tag in Room.Tags)
            {
                GetResponse().AppendStringWithBreak(Tag);
            }

            GetResponse().AppendInt32(Room.UsersWithRights.Count); // users /w rights count

            foreach (uint UserId in Room.UsersWithRights)
            {
                GetResponse().AppendUInt(UserId);
                GetResponse().AppendStringWithBreak(UberEnvironment.GetGame().GetClientManager().GetNameById(UserId));
            }

            GetResponse().AppendInt32(Room.UsersWithRights.Count); // users /w rights count

            GetResponse().AppendBoolean(Room.AllowPets); // allows pets in room - pet system lacking, so always off
            GetResponse().AppendBoolean(Room.AllowPetsEating); // allows pets to eat your food - pet system lacking, so always off
            GetResponse().AppendBoolean(Room.AllowWalkthrough);
            GetResponse().AppendBoolean(Room.Hidewall);

            SendResponse();
        }

        private void UpdateRoomThumbnailMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            int Junk = Request.PopWiredInt32(); // always 3

            Dictionary<int, int> Items = new Dictionary<int, int>();

            int Background = Request.PopWiredInt32();
            int TopLayer = Request.PopWiredInt32();
            int AmountOfItems = Request.PopWiredInt32();

            for (int i = 0; i < AmountOfItems; i++)
            {
                int Pos = Request.PopWiredInt32();
                int Item = Request.PopWiredInt32();

                if (Pos < 0 || Pos > 10)
                {
                    return;
                }

                if (Item < 1 || Item > 27)
                {
                    return;
                }

                if (Items.ContainsKey(Pos))
                {
                    return;
                }

                Items.Add(Pos, Item);
            }

            if (Background < 1 || Background > 24)
            {
                return;
            }

            if (TopLayer < 0 || TopLayer > 11)
            {
                return;
            }

            StringBuilder FormattedItems = new StringBuilder();
            int j = 0;

            foreach (KeyValuePair<int, int> Item in Items)
            {
                if (j > 0)
                {
                    FormattedItems.Append("|");
                }

                FormattedItems.Append(Item.Key + "," + Item.Value);

                j++;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE rooms SET icon_bg = '" + Background + "', icon_fg = '" + TopLayer + "', icon_items = '" + FormattedItems.ToString() + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            Room.Icon = new RoomIcon(Background, TopLayer, Items);

            GetResponse().Init(457);
            GetResponse().AppendUInt(Room.RoomId);
            GetResponse().AppendBoolean(true);
            SendResponse();

            GetResponse().Init(456);
            GetResponse().AppendUInt(Room.RoomId);
            SendResponse();

            RoomData Data = new RoomData();
            Data.Fill(Room);

            GetResponse().Init(454);
            GetResponse().AppendBoolean(false);
            Data.Serialize(GetResponse(), false);
            SendResponse();
        }

        private void SaveRoomSettingsMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            int Id = Request.PopWiredInt32();
            string Name = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            string Description = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            int State = Request.PopWiredInt32();
            string Password = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            int MaxUsers = Request.PopWiredInt32();
            int CategoryId = Request.PopWiredInt32();
            int TagCount = Request.PopWiredInt32();

            List<string> Tags = new List<string>();
            StringBuilder formattedTags = new StringBuilder();

            for (int i = 0; i < TagCount; i++)
            {
                if (i > 0)
                {
                    formattedTags.Append(",");
                }

                string tag = UberEnvironment.FilterInjectionChars(Request.PopFixedString().ToLower());

                Tags.Add(tag);
                formattedTags.Append(tag);
            }

            int AllowPets = 0;
            int AllowPetsEat = 0;
            int AllowWalkthrough = 0;
            int Hidewall = 0;

            string _AllowPets = Request.PlainReadBytes(1)[0].ToString();
            Request.AdvancePointer(1);

            string _AllowPetsEat = Request.PlainReadBytes(1)[0].ToString();
            Request.AdvancePointer(1);

            string _AllowWalkthrough = Request.PlainReadBytes(1)[0].ToString();
            Request.AdvancePointer(1);

            string _Hidewall = Request.PlainReadBytes(1)[0].ToString();
            Request.AdvancePointer(1);

            if (Name.Length < 1)
            {
                return;
            }

            if (State < 0 || State > 2)
            {
                return;
            }

            if (MaxUsers != 10 && MaxUsers != 15 && MaxUsers != 20 && MaxUsers != 25)
            {
                return;
            }

            FlatCat FlatCat = UberEnvironment.GetGame().GetNavigator().GetFlatCat(CategoryId);

            if (FlatCat == null)
            {
                return;
            }

            if (FlatCat.MinRank > Session.GetHabbo().Rank)
            {
                Session.SendNotif("You are not allowed to use this category. Your room has been moved to no category instead.");
                CategoryId = 0;
            }

            if (TagCount > 2)
            {
                return;
            }

            if (State < 0 || State > 2)
            {
                return;
            }

            if (_AllowPets == "65")
            {
                AllowPets = 1;
                Room.AllowPets = true;
            }
            else
            {
                Room.AllowPets = false;
            }

            if (_AllowPetsEat == "65")
            {
                AllowPetsEat = 1;
                Room.AllowPetsEating = true;
            }
            else
            {
                Room.AllowPetsEating = false;
            }

            if (_AllowWalkthrough == "65")
            {
                AllowWalkthrough = 1;
                Room.AllowWalkthrough = true;
            }
            else
            {
                Room.AllowWalkthrough = false;
            }

            if (_Hidewall == "65")
            {
                Hidewall = 1;
                Room.Hidewall = true;
            }
            else
            {
                Room.Hidewall = false;
            }

            Room.Name = Name;
            Room.State = State;
            Room.Description = Description;
            Room.Category = CategoryId;
            Room.Password = Password;
            Room.Tags = Tags;
            Room.UsersMax = MaxUsers;

            string formattedState = "open";

            if (Room.State == 1)
            {
                formattedState = "locked";
            }
            else if (Room.State > 1)
            {
                formattedState = "password";
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("caption", Room.Name);
                dbClient.AddParamWithValue("description", Room.Description);
                dbClient.AddParamWithValue("password", Room.Password);
                dbClient.AddParamWithValue("tags", formattedTags.ToString());
                dbClient.ExecuteQuery("UPDATE rooms SET caption = @caption, description = @description, password = @password, category = '" + CategoryId + "', state = '" + formattedState + "', tags = @tags, users_max = '" + MaxUsers + "', allow_pets = '" + AllowPets + "', allow_pets_eat = '" + AllowPetsEat + "', allow_walkthrough = '" + AllowWalkthrough + "', allow_hidewall = '" + Hidewall + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            GetResponse().Init(467);
            GetResponse().AppendUInt(Room.RoomId);
            SendResponse();

            GetResponse().Init(456);
            GetResponse().AppendUInt(Room.RoomId);
            SendResponse();

            GetResponse().Init(472);
            GetResponse().AppendBoolean(Room.Hidewall);
            UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).SendMessage(Response);

            RoomData Data = new RoomData();
            Data.Fill(Room);

            GetResponse().Init(454);
            GetResponse().AppendBoolean(false);
            Data.Serialize(GetResponse(), false);
            SendResponse();
        }

        private void AssignRightsMessageEvent()
        {
            uint UserId = Request.PopWiredUInt();

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            RoomUser RoomUser = Room.GetRoomUserByHabbo(UserId);

            if (Room == null || !Room.CheckRights(Session, true) || RoomUser == null || RoomUser.IsBot)
            {
                return;
            }

            if (Room.UsersWithRights.Contains(UserId))
            {
                // todo: fix silly bug
                Session.SendNotif("User already has rights! (There appears to be a bug with the rights button, we are looking into it - for now rely on 'Advanced settings')");
                return;
            }

            Room.UsersWithRights.Add(UserId);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("INSERT INTO room_rights (room_id,user_id) VALUES ('" + Room.RoomId + "','" + UserId + "')");
            }

            GetResponse().Init(510);
            GetResponse().AppendUInt(Room.RoomId);
            GetResponse().AppendUInt(UserId);
            GetResponse().AppendStringWithBreak(RoomUser.GetClient().GetHabbo().Username);
            SendResponse();

            RoomUser.AddStatus("flatcrtl", "");
            RoomUser.UpdateNeeded = true;

            RoomUser.GetClient().SendPacket(new ServerPacket(42));

            // G~hntX]h_u@UMeth0d9
        }

        private void RemoveRightsMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            StringBuilder DeleteParams = new StringBuilder();

            int Amount = Request.PopWiredInt32();

            for (int i = 0; i < Amount; i++)
            {
                if (i > 0)
                {
                    DeleteParams.Append(" OR ");
                }

                uint UserId = Request.PopWiredUInt();
                Room.UsersWithRights.Remove(UserId);
                DeleteParams.Append("room_id = '" + Room.RoomId + "' AND user_id = '" + UserId + "'");

                RoomUser User = Room.GetRoomUserByHabbo(UserId);

                if (User != null && !User.IsBot)
                {
                    User.GetClient().SendPacket(new ServerPacket(43));
                }

                // GhntX]hqu@U
                GetResponse().Init(511);
                GetResponse().AppendUInt(Room.RoomId);
                GetResponse().AppendUInt(UserId);
                SendResponse();
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM room_rights WHERE " + DeleteParams.ToString());
            }
        }

        private void RemoveAllRightsMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            foreach (uint UserId in Room.UsersWithRights)
            {
                RoomUser User = Room.GetRoomUserByHabbo(UserId);

                if (User != null && !User.IsBot)
                {
                    User.GetClient().SendPacket(new ServerPacket(43));
                }

                GetResponse().Init(511);
                GetResponse().AppendUInt(Room.RoomId);
                GetResponse().AppendUInt(UserId);
                SendResponse();
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM room_rights WHERE room_id = '" + Room.RoomId + "'");
            }

            Room.UsersWithRights.Clear();
        }

        private void KickUserMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            if (!Room.CheckRights(Session))
            {
                return; // insufficient permissions
            }

            uint UserId = Request.PopWiredUInt();
            RoomUser User = Room.GetRoomUserByHabbo(UserId);

            if (User == null || User.IsBot)
            {
                return;
            }

            if (Room.CheckRights(User.GetClient(), true) || User.GetClient().GetHabbo().HasFuse("fuse_mod"))
            {
                return; // can't kick room owner or mods!
            }

            Room.RemoveUserFromRoom(User.GetClient(), true, true);
        }

        private void BanUserMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return; // insufficient permissions
            }

            uint UserId = Request.PopWiredUInt();
            RoomUser User = Room.GetRoomUserByHabbo(UserId);

            if (User == null || User.IsBot)
            {
                return;
            }

            if (User.GetClient().GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            Room.AddBan(UserId);
            Room.RemoveUserFromRoom(User.GetClient(), true, true);
        }

        private void UpdateNavigatorSettingsMessageEvent()
        {
            uint RoomId = Request.PopWiredUInt();
            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (RoomId != 0)
            {
                if (Data == null || Data.Owner.ToLower() != Session.GetHabbo().Username.ToLower())
                {
                    return;
                }
            }

            Session.GetHabbo().HomeRoom = RoomId;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE users SET home_room = '" + RoomId + "' WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            GetResponse().Init(455);
            GetResponse().AppendUInt(RoomId);
            SendResponse();
        }

        private void DeleteRoomMessageEvent()
        {
            uint RoomId = Request.PopWiredUInt();
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

            GetResponse().Init(101);
            SendResponse();

            Session.SendPacket(UberEnvironment.GetGame().GetNavigator().SerializeRoomListing(Session, -3));
        }

        private void LookToMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            User.Unidle();

            int X = Request.PopWiredInt32();
            int Y = Request.PopWiredInt32();

            if (X == User.X && Y == User.Y)
            {
                return;
            }

            int Rot = Rotation.Calculate(User.X, User.Y, X, Y);

            User.SetRot(Rot);
        }

        private void StartTypingMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            ServerPacket Message = new ServerPacket(361);
            Message.AppendInt32(User.VirtualId);
            Message.AppendBoolean(true);
            Room.SendMessage(Message);
        }

        private void CancelTypingMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            ServerPacket Message = new ServerPacket(361);
            Message.AppendInt32(User.VirtualId);
            Message.AppendBoolean(false);
            Room.SendMessage(Message);
        }

        private void IgnoreUserMessageEvent()
        {
            /*Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoom);

            if (Room == null)
            {
                return;
            }

            uint Id = Request.PopWiredUInt();

            if (Session.GetHabbo().MutedUsers.Contains(Id))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Add(Id);

            GetResponse().Init(419);
            GetResponse().AppendInt32(1);
            SendResponse();*/
        }

        private void UnignoreUserMessageEvent()
        {
            /*Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoom);

            if (Room == null)
            {
                return;
            }

            uint Id = Request.PopWiredUInt();

            if (!Session.GetHabbo().MutedUsers.Contains(Id))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Remove(Id);

            GetResponse().Init(419);
            GetResponse().AppendInt32(3);
            SendResponse();*/
        }

        private void CanCreateFlatMessageEventMessageEventEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            Boolean Allow = true;
            int ErrorCode = 0;

            if (Room.State != 0)
            {
                Allow = false;
                ErrorCode = 3;
            }

            GetResponse().Init(367);
            GetResponse().AppendBoolean(Allow);
            GetResponse().AppendInt32(ErrorCode);
            SendResponse();
        }

        private void CreateEventMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.Event != null || Room.State != 0)
            {
                return;
            }

            int category = Request.PopWiredInt32();
            string name = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            string descr = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            int tagCount = Request.PopWiredInt32();

            Room.Event = new RoomEvent(Room.RoomId, name, descr, category, null);
            Room.Event.Tags = new List<string>();

            for (int i = 0; i < tagCount; i++)
            {
                Room.Event.Tags.Add(UberEnvironment.FilterInjectionChars(Request.PopFixedString()));
            }

            Room.SendMessage(Room.Event.Serialize(Session));
        }

        private void CancelEventMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.Event == null)
            {
                return;
            }

            Room.Event = null;

            ServerPacket Message = new ServerPacket(370);
            Message.AppendStringWithBreak("-1");
            Room.SendMessage(Message);
        }

        private void EditEventMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.Event == null)
            {
                return;
            }

            int category = Request.PopWiredInt32();
            string name = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            string descr = UberEnvironment.FilterInjectionChars(Request.PopFixedString());
            int tagCount = Request.PopWiredInt32();

            Room.Event.Category = category;
            Room.Event.Name = name;
            Room.Event.Description = descr;
            Room.Event.Tags = new List<string>();

            for (int i = 0; i < tagCount; i++)
            {
                Room.Event.Tags.Add(UberEnvironment.FilterInjectionChars(Request.PopFixedString()));
            }

            Room.SendMessage(Room.Event.Serialize(Session));
        }

        private void WaveMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            User.Unidle();

            User.DanceId = 0;

            ServerPacket Message = new ServerPacket(481);
            Message.AppendInt32(User.VirtualId);
            Room.SendMessage(Message);
        }

        private void GetUserTagsMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Request.PopWiredUInt());

            if (User == null || User.IsBot)
            {
                return;
            }

            GetResponse().Init(350);
            GetResponse().AppendUInt(User.GetClient().GetHabbo().Id);
            GetResponse().AppendInt32(User.GetClient().GetHabbo().Tags.Count);

            foreach (string Tag in User.GetClient().GetHabbo().Tags)
            {
                GetResponse().AppendStringWithBreak(Tag);
            }

            SendResponse();
        }

        private void GetSelectedBadgesMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Request.PopWiredUInt());

            if (User == null || User.IsBot)
            {
                return;
            }
            GetResponse().Init(228);
            GetResponse().AppendUInt(User.GetClient().GetHabbo().Id);
            GetResponse().AppendInt32(User.GetClient().GetHabbo().GetBadgeComponent().EquippedCount);

            foreach (Badge Badge in User.GetClient().GetHabbo().GetBadgeComponent().BadgeList)
            {
                if (Badge.Slot <= 0)
                {
                    continue;
                }

                GetResponse().AppendInt32(Badge.Slot);
                GetResponse().AppendStringWithBreak(Badge.Code);
            }

            SendResponse();
        }

        private void RateFlatMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || Session.GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.CheckRights(Session, true))
            {
                return;
            }

            int Rating = Request.PopWiredInt32();

            switch (Rating)
            {
                case -1:

                    Room.Score--;
                    break;

                case 1:

                    Room.Score++;
                    break;

                default:

                    return;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE rooms SET score = '" + Room.Score + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            Session.GetHabbo().RatedRooms.Add(Room.RoomId);

            GetResponse().Init(345);
            GetResponse().AppendInt32(Room.Score);
            SendResponse();
        }

        private void DanceMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            User.Unidle();

            int DanceMessageEventId = Request.PopWiredInt32();

            if (DanceMessageEventId < 0 || DanceMessageEventId > 4 || (!Session.GetHabbo().HasFuse("fuse_use_club_dance") && DanceMessageEventId > 1))
            {
                DanceMessageEventId = 0;
            }

            if (DanceMessageEventId > 0 && User.CarryItemID > 0)
            {
                User.CarryItem(0);
            }

            User.DanceId = DanceMessageEventId;

            ServerPacket DanceMessageEventMessage = new ServerPacket(480);
            DanceMessageEventMessage.AppendInt32(User.VirtualId);
            DanceMessageEventMessage.AppendInt32(DanceMessageEventId);
            Room.SendMessage(DanceMessageEventMessage);
        }

        private void LetUserInMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            string Name = Request.PopFixedString();
            byte[] Result = Request.ReadBytes(1);

            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Name);

            if (Client == null)
            {
                return;
            }

            if (Result[0] == Convert.ToByte(65))
            {
                Client.GetHabbo().LoadingChecksPassed = true;

                Client.GetMessageHandler().GetResponse().Init(41);
                Client.GetMessageHandler().SendResponse();
            }
            else
            {
                Client.GetMessageHandler().GetResponse().Init(131);
                Client.GetMessageHandler().SendResponse();
            }
        }

        private void ApplyRoomEffect()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(Request.PopWiredUInt());

            if (Item == null)
            {
                return;
            }

            string type = "floor";

            if (Item.GetBaseItem().Name.ToLower().Contains("wallpaper"))
            {
                type = "wallpaper";
            }
            else if (Item.GetBaseItem().Name.ToLower().Contains("landscape"))
            {
                type = "landscape";
            }

            switch (type)
            {
                case "floor":

                    Room.Floor = Item.ExtraData;
                    break;

                case "wallpaper":

                    Room.Wallpaper = Item.ExtraData;
                    break;

                case "landscape":

                    Room.Landscape = Item.ExtraData;
                    break;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE rooms SET " + type + " = '" + Item.ExtraData + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

            ServerPacket Message = new ServerPacket(46);
            Message.AppendStringWithBreak(type);
            Message.AppendStringWithBreak(Item.ExtraData);
            Room.SendMessage(Message);
        }

        private void PlaceObjectMessageEvent()
        {
            // AZ@J16 10 10 0

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            string PlacementData = Request.PopFixedString();
            string[] DataBits = PlacementData.Split(' ');
            uint ItemId = uint.Parse(DataBits[0]);

            UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);

            if (Item == null)
            {
                return;
            }

            switch (Item.GetBaseItem().InteractionType.ToLower())
            {
                case "dimmer":

                    if (Room.ItemCountByType("dimmer") >= 1)
                    {
                        Session.SendNotif("You can only have one moodlight in a room.");
                        return;
                    }

                    break;
            }

            // Wall Item
            if (DataBits[1].StartsWith(":"))
            {
                string WallPos = Room.WallPositionCheck(":" + PlacementData.Split(':')[1]);

                if (WallPos == null)
                {
                    GetResponse().Init(516);
                    GetResponse().AppendInt32(11);
                    SendResponse();

                    return;
                }

                RoomItem RoomItem = new RoomItem(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, 0, 0, 0.0, 0, WallPos);

                if (Room.SetWallItem(Session, RoomItem))
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
                }
            }
            // Floor Item
            else
            {
                int X = int.Parse(DataBits[1]);
                int Y = int.Parse(DataBits[2]);
                int Rot = int.Parse(DataBits[3]);

                RoomItem RoomItem = new RoomItem(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, 0, 0, 0, 0, "");

                if (Room.SetFloorItem(Session, RoomItem, X, Y, Rot, true))
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
                }
            }
        }

        private void PickupObjectMessageEvent()
        {
            int junk = Request.PopWiredInt32();

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            RoomItem Item = Room.GetItem(Request.PopWiredUInt());

            if (Item == null)
            {
                return;
            }

            switch (Item.GetBaseItem().InteractionType.ToLower())
            {
                case "postit":

                    return; // not allowed to pick up post.its
            }

            Room.RemoveFurniture(Session, Item.Id);
            Session.GetHabbo().GetInventoryComponent().AddItem(Item.Id, Item.BaseItem, Item.ExtraData);
            Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
        }

        private void MoveAvatarMessageEventItem()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            RoomItem Item = Room.GetItem(Request.PopWiredUInt());

            if (Item == null)
            {
                return;
            }

            int x = Request.PopWiredInt32();
            int y = Request.PopWiredInt32();
            int Rotation = Request.PopWiredInt32();
            int Junk = Request.PopWiredInt32();

            Room.SetFloorItem(Session, Item, x, y, Rotation, false);
        }

        private void UseFurnitureMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomItem Item = Room.GetItem(Request.PopWiredUInt());

            if (Item == null)
            {
                return;
            }

            Boolean hasRights = false;

            if (Room.CheckRights(Session))
            {
                hasRights = true;
            }

            Item.Interactor.OnTrigger(Session, Item, Request.PopWiredInt32(), hasRights);
        }

        private void UseFurnitureMessageEventDiceSpecial()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomItem Item = Room.GetItem(Request.PopWiredUInt());

            if (Item == null)
            {
                return;
            }

            Boolean hasRights = false;

            if (Room.CheckRights(Session))
            {
                hasRights = true;
            }

            Item.Interactor.OnTrigger(Session, Item, -1, hasRights);
        }

        private void GetItemDataMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomItem Item = Room.GetItem(Request.PopWiredUInt());

            if (Item == null || Item.GetBaseItem().InteractionType.ToLower() != "postit")
            {
                return;
            }

            // @p181855059CFF9C stickynotemsg
            GetResponse().Init(48);
            GetResponse().AppendStringWithBreak(Item.Id.ToString());
            GetResponse().AppendStringWithBreak(Item.ExtraData);
            SendResponse();
        }

        private void SetItemDataMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomItem Item = Room.GetItem(Request.PopWiredUInt());

            if (Item == null || Item.GetBaseItem().InteractionType.ToLower() != "postit")
            {
                return;
            }

            String Data = Request.PopFixedString();
            String Color = Data.Split(' ')[0];
            String Text = UberEnvironment.FilterInjectionChars(Data.Substring(Color.Length + 1), true);

            if (!Room.CheckRights(Session))
            {
                if (!Data.StartsWith(Item.ExtraData))
                {
                    return; // we can only ADD stuff! older stuff changed, this is not allowed
                }
            }

            switch (Color)
            {
                case "FFFF33":
                case "FF9CFF":
                case "9CCEFF":
                case "9CFF9C":

                    break;

                default:

                    return; // invalid color
            }

            Item.ExtraData = Color + " " + Text;
            Item.UpdateState(true, true);
        }

        private void RemoveItemMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            RoomItem Item = Room.GetItem(Request.PopWiredUInt());

            if (Item == null || Item.GetBaseItem().InteractionType.ToLower() != "postit")
            {
                return;
            }

            Room.RemoveFurniture(Session, Item.Id);
        }

        private void PresentOpenMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            RoomItem Present = Room.GetItem(Request.PopWiredUInt());

            if (Present == null)
            {
                return;
            }

            DataRow Data = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Data = dbClient.ReadDataRow("SELECT base_id,amount,extra_data FROM user_presents WHERE item_id = '" + Present.Id + "' LIMIT 1");
            }

            if (Data == null)
            {
                return;
            }

            Item BaseItem = UberEnvironment.GetGame().GetItemManager().GetItem((uint)Data["base_id"]);

            if (BaseItem == null)
            {
                return;
            }

            Room.RemoveFurniture(Session, Present.Id);

            GetResponse().Init(219);
            GetResponse().AppendUInt(Present.Id);
            SendResponse();

            GetResponse().Init(129);
            GetResponse().AppendStringWithBreak(BaseItem.Type);
            GetResponse().AppendInt32(BaseItem.SpriteId);
            GetResponse().AppendStringWithBreak(BaseItem.Name);
            SendResponse();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM user_presents WHERE item_id = '" + Present.Id + "' LIMIT 1");
            }

            UberEnvironment.GetGame().GetCatalog().DeliverItems(Session, BaseItem, (int)Data["amount"], (String)Data["extra_data"]);
        }

        private void RoomDimmerGetPresetsMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.MoodlightData == null)
            {
                return;
            }

            GetResponse().Init(365);
            GetResponse().AppendInt32(Room.MoodlightData.Presets.Count);
            GetResponse().AppendInt32(Room.MoodlightData.CurrentPreset);

            int i = 0;

            foreach (MoodlightPreset Preset in Room.MoodlightData.Presets)
            {
                i++;

                GetResponse().AppendInt32(i);
                GetResponse().AppendInt32(int.Parse(UberEnvironment.BoolToEnum(Preset.BackgroundOnly)) + 1);
                GetResponse().AppendStringWithBreak(Preset.ColorCode);
                GetResponse().AppendInt32(Preset.ColorIntensity);
            }

            SendResponse();
        }

        private void RoomDimmerSavePresetMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.MoodlightData == null)
            {
                return;
            }

            RoomItem Item = null;

            foreach (RoomItem I in Room.Items)
            {
                if (I.GetBaseItem().InteractionType.ToLower() == "dimmer")
                {
                    Item = I;
                    break;
                }
            }

            if (Item == null)
            {
                return;
            }

            int Preset = Request.PopWiredInt32();
            int BackgroundMode = Request.PopWiredInt32();
            string ColorCode = Request.PopFixedString();
            int Intensity = Request.PopWiredInt32();

            bool BackgroundOnly = false;

            if (BackgroundMode >= 2)
            {
                BackgroundOnly = true;
            }

            Room.MoodlightData.Enabled = true;
            Room.MoodlightData.CurrentPreset = Preset;
            Room.MoodlightData.UpdatePreset(Preset, ColorCode, Intensity, BackgroundOnly);

            Item.ExtraData = Room.MoodlightData.GenerateExtraData();
            Item.UpdateState();
        }

        private void RoomDimmerChangeStateMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true) || Room.MoodlightData == null)
            {
                return;
            }

            RoomItem Item = null;
            foreach (RoomItem I in Room.Items)
            {
                if (I.GetBaseItem().InteractionType.ToLower() == "dimmer")
                {
                    Item = I;
                    break;
                }
            }

            if (Item == null)
            {
                return;
            }

            if (Room.MoodlightData.Enabled)
            {
                Room.MoodlightData.Disable();
            }
            else
            {
                Room.MoodlightData.Enable();
            }

            Item.ExtraData = Room.MoodlightData.GenerateExtraData();
            Item.UpdateState();
        }

        private void OpenTradingEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CanTradeInRoom)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser User2 = Room.GetRoomUserByVirtualId(Request.PopWiredInt32());

            Room.TryStartTrade(User, User2);
        }

        private void AddItemToTradeEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CanTradeInRoom)
            {
                return;
            }

            Trade Trade = Room.GetUserTrade(Session.GetHabbo().Id);
            UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(Request.PopWiredUInt());

            if (Trade == null || Item == null)
            {
                return;
            }

            Trade.OfferItem(Session.GetHabbo().Id, Item);
        }

        private void RemoveItemFromTradeEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CanTradeInRoom)
            {
                return;
            }

            Trade Trade = Room.GetUserTrade(Session.GetHabbo().Id);
            UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(Request.PopWiredUInt());

            if (Trade == null || Item == null)
            {
                return;
            }

            Trade.TakeBackItem(Session.GetHabbo().Id, Item);
        }

        private void CloseTradingEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CanTradeInRoom)
            {
                return;
            }

            Room.TryStopTrade(Session.GetHabbo().Id);
        }

        private void AcceptTradingEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CanTradeInRoom)
            {
                return;
            }

            Trade Trade = Room.GetUserTrade(Session.GetHabbo().Id);

            if (Trade == null)
            {
                return;
            }

            Trade.Accept(Session.GetHabbo().Id);
        }

        private void UnacceptTradingEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CanTradeInRoom)
            {
                return;
            }

            Trade Trade = Room.GetUserTrade(Session.GetHabbo().Id);

            if (Trade == null)
            {
                return;
            }

            Trade.Unaccept(Session.GetHabbo().Id);
        }

        private void ConfirmAcceptTradingEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CanTradeInRoom)
            {
                return;
            }

            Trade Trade = Room.GetUserTrade(Session.GetHabbo().Id);

            if (Trade == null)
            {
                return;
            }

            Trade.Accept(Session.GetHabbo().Id);
        }

        private void RespectUserMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || Session.GetHabbo().DailyRespectPoints <= 0)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Request.PopWiredUInt());

            if (User == null || User.GetClient().GetHabbo().Id == Session.GetHabbo().Id || User.IsBot)
            {
                return;
            }

            Session.GetHabbo().DailyRespectPoints--;
            User.GetClient().GetHabbo().Respect++;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE users SET respect = respect + 1 WHERE id = '" + User.GetClient().GetHabbo().Id + "' LIMIT 1");
                dbClient.ExecuteQuery("UPDATE users SET daily_respect_points = daily_respect_points - 1 WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            // FxkqUzYP_
            ServerPacket Message = new ServerPacket(440);
            Message.AppendUInt(User.GetClient().GetHabbo().Id);
            Message.AppendInt32(User.GetClient().GetHabbo().Respect);
            Room.SendMessage(Message);
        }

        private void AvatarEffectSelectedEvent()
        {
            Session.GetHabbo().GetAvatarEffectsInventoryComponent().ApplyEffect(Request.PopWiredInt32());
        }

        private void AvatarEffectActivatedEvent()
        {
            Session.GetHabbo().GetAvatarEffectsInventoryComponent().EnableEffect(Request.PopWiredInt32());
        }

        private void RecycleItemsMessageEvent()
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int itemCount = Request.PopWiredInt32();

            if (itemCount != 5)
            {
                return;
            }

            for (int i = 0; i < itemCount; i++)
            {
                UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(Request.PopWiredUInt());

                if (Item != null && Item.GetBaseItem().AllowRecycle)
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);
                }
                else
                {
                    return;
                }
            }

            uint newItemId = UberEnvironment.GetGame().GetCatalog().GenerateItemId();
            EcotronReward Reward = UberEnvironment.GetGame().GetCatalog().GetRandomEcotronReward();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("INSERT INTO user_items (id,user_id,base_item,extra_data) VALUES ('" + newItemId + "','" + Session.GetHabbo().Id + "','1478','" + DateTime.Now.ToLongDateString() + "')");
                dbClient.ExecuteQuery("INSERT INTO user_presents (item_id,base_id,amount,extra_data) VALUES ('" + newItemId + "','" + Reward.BaseId + "','1','')");
            }

            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

            GetResponse().Init(508);
            GetResponse().AppendBoolean(true);
            GetResponse().AppendUInt(newItemId);
            SendResponse();
        }

        private void CreditFurniRedeemMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            RoomItem Exchange = Room.GetItem(Request.PopWiredUInt());

            if (Exchange == null)
            {
                return;
            }

            if (!Exchange.GetBaseItem().Name.StartsWith("CF_") && !Exchange.GetBaseItem().Name.StartsWith("CFC_"))
            {
                return;
            }

            string[] Split = Exchange.GetBaseItem().Name.Split('_');
            int Value = int.Parse(Split[1]);

            if (Value > 0)
            {
                Session.GetHabbo().Credits += Value;
                Session.GetHabbo().UpdateCreditsBalance(true);
            }

            Room.RemoveFurniture(null, Exchange.Id);

            GetResponse().Init(219);
            SendResponse();
        }

        private void TryBusMessageEvent()
        {
            // AQThe Infobus is currently closed.
            GetResponse().Init(81);
            GetResponse().AppendStringWithBreak("The Uber Infobus is not yet in use.");
            SendResponse();
        }

        private void KickBotMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            RoomUser Bot = Room.GetRoomUserByVirtualId(Request.PopWiredInt32());

            if (Bot == null || !Bot.IsBot)
            {
                return;
            }

            Room.RemoveBot(Bot.VirtualId, true);
        }

        private void PlacePetMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || (!Room.AllowPets && !Room.CheckRights(Session, true)))
            {
                return;
            }

            uint PetId = Request.PopWiredUInt();

            Pet Pet = Session.GetHabbo().GetInventoryComponent().GetPet(PetId);

            if (Pet == null || Pet.PlacedInRoom)
            {
                return;
            }

            int X = Request.PopWiredInt32();
            int Y = Request.PopWiredInt32();

            if (!Room.CanWalk(X, Y, 0, true))
            {
                return;
            }

            if (Room.PetCount >= UberEnvironment.GetGame().GetRoomManager().MAX_PETS_PER_ROOM)
            {
                Session.SendNotif("There are too many pets in this room. A room may only contain up to " + UberEnvironment.GetGame().GetRoomManager().MAX_PETS_PER_ROOM + " pets.");
                return;
            }

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.RoomId;

            RoomUser PetUser = Room.DeployBot(new HabboHotel.RoomBots.RoomBot(Pet.PetId, Pet.RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, X, Y, 0, 0, 0, 0, 0, 0), Pet);

            if (Room.CheckRights(Session, true))
            {
                Session.GetHabbo().GetInventoryComponent().MovePetToRoom(Pet.PetId, Room.RoomId);
            }
        }

        private void GetPetInfoMessageEvent()
        {
            uint PetId = Request.PopWiredUInt();

            DataRow Row = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("petid", PetId);
                Row = dbClient.ReadDataRow("SELECT * FROM user_pets WHERE id = @petid LIMIT 1");
            }

            if (Row == null)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetCatalog().GeneratePetFromRow(Row).SerializeInfo());
        }

        private void RemovePetFromFlatMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || Room.IsPublic || (!Room.AllowPets && !Room.CheckRights(Session, true)))
            {
                return;
            }

            uint PetId = Request.PopWiredUInt();
            RoomUser PetUser = Room.GetPet(PetId);

            if (PetUser == null || PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetHabbo().Id)
            {
                return;
            }

            Session.GetHabbo().GetInventoryComponent().AddPet(PetUser.PetData);
            Room.RemoveBot(PetUser.VirtualId, false);
        }

        private void RespectPetMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || Room.IsPublic || (!Room.AllowPets && !Room.CheckRights(Session, true)))
            {
                return;
            }

            uint PetId = Request.PopWiredUInt();
            RoomUser PetUser = Room.GetPet(PetId);

            if (PetUser == null || PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetHabbo().Id)
            {
                return;
            }

            PetUser.PetData.OnRespect();
            Session.GetHabbo().DailyPetRespectPoints--;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("userid", Session.GetHabbo().Id);
                dbClient.ExecuteQuery("UPDATE users SET daily_pet_respect_points = daily_pet_respect_points - 1 WHERE id = @userid LIMIT 1");
            }
        }

        private void GetPetCommandsMessageEvent()
        {
            uint PetID = Request.PopWiredUInt();
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            RoomUser PetUser = Room.GetPet(PetID);
            GetResponse().Init(605);
            GetResponse().AppendUInt(PetID);
            int level = PetUser.PetData.Level;
            GetResponse().AppendInt32(level);
            for (int i = 0; level > i; )
            {
                i++;
                GetResponse().AppendInt32(i - 1);
            }
            SendResponse();
        }

        public void RegisterRooms()
        {
            RequestHandlers[391] = new RequestHandler(OpenFlatConnectionMessageEvent);
            RequestHandlers[182] = new RequestHandler(GetInterstitialMessageEvent);
            RequestHandlers[388] = new RequestHandler(GetPublicSpaceCastLibsMessageEvent);
            RequestHandlers[2] = new RequestHandler(OpenConnectionMessageEvent);
            RequestHandlers[230] = new RequestHandler(GetHabboGroupBadgesMessageEvent);
            RequestHandlers[215] = new RequestHandler(GetFurnitureAliasesMessageEvent);
            RequestHandlers[390] = new RequestHandler(GetRoomEntryDataMessageEvent);
            RequestHandlers[126] = new RequestHandler(GetRoomAdMessageEvent);
            RequestHandlers[52] = new RequestHandler(ChatMessageEvent);
            RequestHandlers[55] = new RequestHandler(ShoutMessageEvent);
            RequestHandlers[56] = new RequestHandler(WhisperMessageEvent);
            RequestHandlers[75] = new RequestHandler(MoveAvatarMessageEvent);
            RequestHandlers[387] = new RequestHandler(CanCreateFlatMessageEventMessageEvent);
            RequestHandlers[29] = new RequestHandler(CreateFlatMessageEvent);
            RequestHandlers[400] = new RequestHandler(GetRoomSettingsMessageEvent);
            RequestHandlers[386] = new RequestHandler(UpdateRoomThumbnailMessageEvent);
            RequestHandlers[401] = new RequestHandler(SaveRoomSettingsMessageEvent);
            RequestHandlers[96] = new RequestHandler(AssignRightsMessageEvent);
            RequestHandlers[97] = new RequestHandler(RemoveRightsMessageEvent);
            RequestHandlers[155] = new RequestHandler(RemoveAllRightsMessageEvent);
            RequestHandlers[95] = new RequestHandler(KickUserMessageEvent);
            RequestHandlers[320] = new RequestHandler(BanUserMessageEvent);
            RequestHandlers[71] = new RequestHandler(OpenTradingEvent);
            RequestHandlers[384] = new RequestHandler(UpdateNavigatorSettingsMessageEvent);
            RequestHandlers[23] = new RequestHandler(DeleteRoomMessageEvent);
            RequestHandlers[79] = new RequestHandler(LookToMessageEvent);
            RequestHandlers[317] = new RequestHandler(StartTypingMessageEvent);
            RequestHandlers[318] = new RequestHandler(CancelTypingMessageEvent);
            RequestHandlers[319] = new RequestHandler(IgnoreUserMessageEvent);
            RequestHandlers[322] = new RequestHandler(UnignoreUserMessageEvent);
            RequestHandlers[345] = new RequestHandler(CanCreateFlatMessageEventMessageEventEvent);
            RequestHandlers[346] = new RequestHandler(CreateEventMessageEvent);
            RequestHandlers[347] = new RequestHandler(CancelEventMessageEvent);
            RequestHandlers[348] = new RequestHandler(EditEventMessageEvent);
            RequestHandlers[94] = new RequestHandler(WaveMessageEvent);
            RequestHandlers[263] = new RequestHandler(GetUserTagsMessageEvent);
            RequestHandlers[159] = new RequestHandler(GetSelectedBadgesMessageEvent);
            RequestHandlers[261] = new RequestHandler(RateFlatMessageEvent);
            RequestHandlers[93] = new RequestHandler(DanceMessageEvent);
            RequestHandlers[98] = new RequestHandler(LetUserInMessageEvent);
            RequestHandlers[59] = new RequestHandler(GoToFlatMessageEvent);
            RequestHandlers[66] = new RequestHandler(ApplyRoomEffect);
            RequestHandlers[90] = new RequestHandler(PlaceObjectMessageEvent);
            RequestHandlers[67] = new RequestHandler(PickupObjectMessageEvent);
            RequestHandlers[73] = new RequestHandler(MoveAvatarMessageEventItem);
            RequestHandlers[392] = new RequestHandler(UseFurnitureMessageEvent); // Generic trigger item
            RequestHandlers[393] = new RequestHandler(UseFurnitureMessageEvent); // Generic trigger item
            RequestHandlers[83] = new RequestHandler(GetItemDataMessageEvent);
            RequestHandlers[84] = new RequestHandler(SetItemDataMessageEvent);
            RequestHandlers[85] = new RequestHandler(RemoveItemMessageEvent);
            RequestHandlers[78] = new RequestHandler(PresentOpenMessageEvent);
            RequestHandlers[341] = new RequestHandler(RoomDimmerGetPresetsMessageEvent);
            RequestHandlers[342] = new RequestHandler(RoomDimmerSavePresetMessageEvent);
            RequestHandlers[343] = new RequestHandler(RoomDimmerChangeStateMessageEvent);
            RequestHandlers[72] = new RequestHandler(AddItemToTradeEvent);
            RequestHandlers[405] = new RequestHandler(RemoveItemFromTradeEvent);
            RequestHandlers[70] = new RequestHandler(CloseTradingEvent);
            RequestHandlers[403] = new RequestHandler(CloseTradingEvent);
            RequestHandlers[69] = new RequestHandler(AcceptTradingEvent);
            RequestHandlers[68] = new RequestHandler(UnacceptTradingEvent);
            RequestHandlers[402] = new RequestHandler(ConfirmAcceptTradingEvent);
            RequestHandlers[371] = new RequestHandler(RespectUserMessageEvent);
            RequestHandlers[372] = new RequestHandler(AvatarEffectSelectedEvent);
            RequestHandlers[373] = new RequestHandler(AvatarEffectActivatedEvent);
            RequestHandlers[232] = new RequestHandler(UseFurnitureMessageEvent); // One way gates
            RequestHandlers[314] = new RequestHandler(UseFurnitureMessageEvent); // Love Shuffler
            RequestHandlers[247] = new RequestHandler(UseFurnitureMessageEvent); // Habbo Wheel
            RequestHandlers[76] = new RequestHandler(UseFurnitureMessageEvent); // Dice
            RequestHandlers[77] = new RequestHandler(UseFurnitureMessageEventDiceSpecial); // Dice (special)
            RequestHandlers[414] = new RequestHandler(RecycleItemsMessageEvent);
            RequestHandlers[183] = new RequestHandler(CreditFurniRedeemMessageEvent);
            RequestHandlers[113] = new RequestHandler(TryBusMessageEvent);
            RequestHandlers[441] = new RequestHandler(KickBotMessageEvent);
            RequestHandlers[3002] = new RequestHandler(PlacePetMessageEvent);
            RequestHandlers[3001] = new RequestHandler(GetPetInfoMessageEvent);
            RequestHandlers[3003] = new RequestHandler(RemovePetFromFlatMessageEvent);
            RequestHandlers[3005] = new RequestHandler(RespectPetMessageEvent);
            RequestHandlers[3004] = new RequestHandler(GetPetCommandsMessageEvent);
        }
    }
}