using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uber.Communication.Incoming
{
    public static class ClientPacketHeader
    {
        // _-2Pf[206] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.InitCryptoMessageComposer;
        public const int InitCryptoMessageEvent = 206;

        // _-2Pf[2002] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.GenerateSecretKeyMessageComposer;
        public const int GenerateSecretKeyMessageEvent = 2002;

        // _-2Pf[1170] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.VersionCheckMessageComposer;
        public const int VersionCheckMessageEvent = 1170;

        // _-2Pf[813] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.UniqueIDMessageComposer;
        public const int UniqueIDMessageEvent = 813;

        // _-2Pf[1817] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.GetSessionParametersMessageComposer;
        public const int GetSessionParametersMessageEvent = 1817;

        // _-2Pf[415] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.SSOTicketMessageComposer;
        public const int SSOTicketMessageEvent = 415;

        // _-2Pf[756] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.TryLoginMessageComposer;
        public const int TryLoginMessageEvent = 756;

        // _-2Pf[196] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.PongMessageComposer;
        public const int PongMessageEvent = 196;

        // _-2Pf[7] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.InfoRetrieveMessageComposer;
        public const int InfoRetrieveMessageEvent = 7;

        // _-2Pf[0x0200] =
        // com.sulake.habbo.communication.messages.outgoing.handshake.DisconnectMessageComposer;
        public const int DisconnectMessageEvent = 0x0200;

        // _-2Pf[12] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.MessengerInitMessageComposer;
        public const int MessengerInitMessageEvent = 12;

        // _-2Pf[15] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.FriendListUpdateMessageComposer;
        public const int FriendListUpdateMessageEvent = 15;

        // _-2Pf[33] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.SendMsgMessageComposer;
        public const int SendMsgMessageEvent = 33;

        // _-2Pf[37] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.AcceptBuddyMessageComposer;
        public const int AcceptBuddyMessageEvent = 37;

        // _-2Pf[38] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.DeclineBuddyMessageComposer;
        public const int DeclineBuddyMessageEvent = 38;

        // _-2Pf[39] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.RequestBuddyMessageComposer;
        public const int RequestBuddyMessageEvent = 39;

        // _-2Pf[40] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.RemoveBuddyMessageComposer;
        public const int RemoveBuddyMessageEvent = 40;

        // _-2Pf[41] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.HabboSearchMessageComposer;
        public const int HabboSearchMessageEvent = 41;

        // _-2Pf[233] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.GetBuddyRequestsMessageComposer;
        public const int GetBuddyRequestsMessageEvent = 233;

        // _-2Pf[262] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.FollowFriendMessageComposer;
        public const int FollowFriendMessageEvent = 262;

        // _-2Pf[34] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.SendRoomInviteMessageComposer;
        public const int SendRoomInviteMessageEvent = 34;

        // _-2Pf[490] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.FindNewFriendsMessageComposer;
        public const int FindNewFriendsMessageEvent = 490;

        // _-2Pf[500] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.GetEventStreamComposer;
        public const int GetEventStreamEvent = 500;

        // _-2Pf[501] =
        // com.sulake.habbo.communication.messages.outgoing.friendlist.SetEventStreamingAllowedComposer;
        public const int SetEventStreamingAllowedEvent = 501;

        // _-2Pf[8] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.purse.GetCreditsInfoComposer;
        public const int GetCreditsInfoEvent = 8;

        // _-2Pf[404] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.furni.RequestFurniInventoryComposer;
        public const int RequestFurniInventoryEvent = 404;

        // _-2Pf[372] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.avatareffect.AvatarEffectSelectedComposer;
        public const int AvatarEffectSelectedEvent = 372;

        // _-2Pf[373] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.avatareffect.AvatarEffectActivatedComposer;
        public const int AvatarEffectActivatedEvent = 373;

        // _-2Pf[157] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.badges.GetBadgesComposer;
        public const int GetBadgesEvent = 157;

        // _-2Pf[158] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.badges.SetActivatedBadgesComposer;
        public const int SetActivatedBadgesEvent = 158;

        // _-2Pf[3032] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.badges.GetBadgePointLimitsComposer;
        public const int GetBadgePointLimitsEvent = 3032;

        // _-2Pf[370] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.achievements.GetAchievementsComposer;
        public const int GetAchievementsEvent = 370;

        // _-2Pf[68] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.UnacceptTradingComposer;
        public const int UnacceptTradingEvent = 68;

        // _-2Pf[69] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.AcceptTradingComposer;
        public const int AcceptTradingEvent = 69;

        // _-2Pf[70] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.CloseTradingComposer;
        public const int CloseTradingEvent = 70;

        // _-2Pf[71] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.OpenTradingComposer;
        public const int OpenTradingEvent = 71;

        // _-2Pf[72] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.AddItemToTradeComposer;
        public const int AddItemToTradeEvent = 72;

        // _-2Pf[402] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.ConfirmAcceptTradingComposer;
        public const int ConfirmAcceptTradingEvent = 402;

        // _-2Pf[403] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.ConfirmDeclineTradingComposer;
        public const int ConfirmDeclineTradingEvent = 403;

        // _-2Pf[405] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.trading.RemoveItemFromTradeComposer;
        public const int RemoveItemFromTradeEvent = 405;

        // _-2Pf[3000] =
        // com.sulake.habbo.communication.messages.outgoing.inventory.pets.GetPetInventoryComposer;
        public const int GetPetInventoryEvent = 3000;

        // _-2Pf[3010] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.MakeOfferMessageComposer;
        public const int MakeOfferMessageEvent = 3010;

        // _-2Pf[3011] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.GetMarketplaceConfigurationMessageComposer;
        public const int GetMarketplaceConfigurationMessageEvent = 3011;

        // _-2Pf[3012] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.GetMarketplaceCanMakeOfferComposer;
        public const int GetMarketplaceCanMakeOfferEvent = 3012;

        // _-2Pf[3013] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.BuyMarketplaceTokensMessageComposer;
        public const int BuyMarketplaceTokensMessageEvent = 3013;

        // _-2Pf[3014] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.BuyOfferMessageComposer;
        public const int BuyOfferMessageEvent = 3014;

        // _-2Pf[3015] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.CancelOfferMessageComposer;
        public const int CancelOfferMessageEvent = 3015;

        // _-2Pf[3016] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.RedeemOfferCreditsMessageComposer;
        public const int RedeemOfferCreditsMessageEvent = 3016;

        // _-2Pf[3018] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.GetOffersMessageComposer;
        public const int GetOffersMessageEvent = 3018;

        // _-2Pf[3019] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.GetOwnOffersMessageComposer;
        public const int GetOwnOffersMessageEvent = 3019;

        // _-2Pf[3020] =
        // com.sulake.habbo.communication.messages.outgoing.marketplace.GetMarketplaceItemStatsComposer;
        public const int GetMarketplaceItemStatsEvent = 3020;

        // _-2Pf[20] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.DeleteFavouriteRoomMessageComposer;
        public const int DeleteFavouriteRoomMessageEvent = 20;

        // _-2Pf[19] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.AddFavouriteRoomMessageComposer;
        public const int AddFavouriteRoomMessageEvent = 19;

        // _-2Pf[151] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.GetUserFlatCatsMessageComposer;
        public const int GetUserFlatCatsMessageEvent = 151;

        // _-2Pf[345] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.CanCreateEventMessageComposer;
        public const int CanCreateEventMessageEvent = 345;

        // _-2Pf[346] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.CreateEventMessageComposer;
        public const int CreateEventMessageEvent = 346;

        // _-2Pf[347] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.CancelEventMessageComposer;
        public const int CancelEventMessageEvent = 347;

        // _-2Pf[348] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.EditEventMessageComposer;
        public const int EditEventMessageEvent = 348;

        // _-2Pf[400] =
        // com.sulake.habbo.communication.messages.outgoing.roomsettings.GetRoomSettingsMessageComposer;
        public const int GetRoomSettingsMessageEvent = 400;

        // _-2Pf[401] =
        // com.sulake.habbo.communication.messages.outgoing.roomsettings.SaveRoomSettingsMessageComposer;
        public const int SaveRoomSettingsMessageEvent = 401;

        // _-2Pf[23] =
        // com.sulake.habbo.communication.messages.outgoing.roomsettings.DeleteRoomMessageComposer;
        public const int DeleteRoomMessageEvent = 23;

        // _-2Pf[380] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.GetOfficialRoomsMessageComposer;
        public const int GetOfficialRoomsMessageEvent = 380;

        // _-2Pf[382] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.GetPopularRoomTagsMessageComposer;
        public const int GetPopularRoomTagsMessageEvent = 382;

        // _-2Pf[384] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.UpdateNavigatorSettingsMessageComposer;
        public const int UpdateNavigatorSettingsMessageEvent = 384;

        // _-2Pf[385] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.GetGuestRoomMessageComposer;
        public const int GetGuestRoomMessageEvent = 385;

        // _-2Pf[386] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.UpdateRoomThumbnailMessageComposer;
        public const int UpdateRoomThumbnailMessageEvent = 386;

        // _-2Pf[387] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.CanCreateRoomMessageComposer;
        public const int CanCreateRoomMessageEvent = 387;

        // _-2Pf[29] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.CreateFlatMessageComposer;
        public const int CreateFlatMessageEvent = 29;

        // _-2Pf[261] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.RateFlatMessageComposer;
        public const int RateFlatMessageEvent = 261;

        // _-2Pf[388] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.GetPublicSpaceCastLibsMessageComposer;
        public const int GetPublicSpaceCastLibsMessageEvent = 388;

        // _-2Pf[430] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.PopularRoomsSearchMessageComposer;
        public const int PopularRoomsSearchMessageEvent = 430;

        // _-2Pf[431] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.RoomsWithHighestScoreSearchMessageComposer;
        public const int RoomsWithHighestScoreSearchMessageEvent = 431;

        // _-2Pf[432] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.MyFriendsRoomsSearchMessageComposer;
        public const int MyFriendsRoomsSearchMessageEvent = 432;

        // _-2Pf[433] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.RoomsWhereMyFriendsAreSearchMessageComposer;
        public const int RoomsWhereMyFriendsAreSearchMessageEvent = 433;

        // _-2Pf[434] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.MyRoomsSearchMessageComposer;
        public const int MyRoomsSearchMessageEvent = 434;

        // _-2Pf[435] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.MyFavouriteRoomsSearchMessageComposer;
        public const int MyFavouriteRoomsSearchMessageEvent = 435;

        // _-2Pf[436] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.MyRoomHistorySearchMessageComposer;
        public const int MyRoomHistorySearchMessageEvent = 436;

        // _-2Pf[437] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.RoomTextSearchMessageComposer;
        public const int RoomTextSearchMessageEvent = 437;

        // _-2Pf[438] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.RoomTagSearchMessageComposer;
        public const int RoomTagSearchMessageEvent = 438;

        // _-2Pf[439] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.LatestEventsSearchMessageComposer;
        public const int LatestEventsSearchMessageEvent = 439;

        // _-2Pf[483] =
        // com.sulake.habbo.communication.messages.outgoing.navigator.ToggleStaffPickMessageComposer;
        public const int ToggleStaffPickMessageEvent = 483;

        // _-2Pf[3004] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.GetPetCommandsMessageComposer;
        public const int GetPetCommandsMessageEvent = 3004;

        // _-2Pf[2] =
        // com.sulake.habbo.communication.messages.outgoing.room.session.OpenConnectionMessageComposer;
        public const int OpenConnectionMessageEvent = 2;

        // _-2Pf[391] =
        // com.sulake.habbo.communication.messages.outgoing.room.session.OpenFlatConnectionMessageComposer;
        public const int OpenFlatConnectionMessageEvent = 391;

        // _-2Pf[52] =
        // com.sulake.habbo.communication.messages.outgoing.room.chat.ChatMessageComposer;
        public const int ChatMessageEvent = 52;

        // _-2Pf[55] =
        // com.sulake.habbo.communication.messages.outgoing.room.chat.ShoutMessageComposer;
        public const int ShoutMessageEvent = 55;

        // _-2Pf[56] =
        // com.sulake.habbo.communication.messages.outgoing.room.chat.WhisperMessageComposer;
        public const int WhisperMessageEvent = 56;

        // _-2Pf[317] =
        // com.sulake.habbo.communication.messages.outgoing.room.chat.StartTypingMessageComposer;
        public const int StartTypingMessageEvent = 317;

        // _-2Pf[318] =
        // com.sulake.habbo.communication.messages.outgoing.room.chat.CancelTypingMessageComposer;
        public const int CancelTypingMessageEvent = 318;

        // _-2Pf[104] =
        // com.sulake.habbo.communication.messages.outgoing.room.avatar.SignMessageComposer;
        public const int SignMessageEvent = 104;

        // _-2Pf[484] =
        // com.sulake.habbo.communication.messages.outgoing.room.avatar.ChangeMottoMessageComposer;
        public const int ChangeMottoMessageEvent = 484;

        // _-2Pf[59] =
        // com.sulake.habbo.communication.messages.outgoing.room.session.GoToFlatMessageComposer;
        public const int GoToFlatMessageEvent = 59;

        // _-2Pf[390] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.GetRoomEntryDataMessageComposer;
        public const int GetRoomEntryDataMessageEvent = 390;

        // _-2Pf[392] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.UseFurnitureMessageComposer;
        public const int UseFurnitureMessageEvent = 392;

        // _-2Pf[67] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.PickupObjectMessageComposer;
        public const int PickupObjectMessageEvent = 67;

        // _-2Pf[73] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.MoveObjectMessageComposer;
        public const int MoveObjectMessageEvent = 73;

        // _-2Pf[74] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.SetObjectDataMessageComposer;
        public const int SetObjectDataMessageEvent = 74;

        // _-2Pf[75] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.MoveAvatarMessageComposer;
        public const int MoveAvatarMessageEvent = 75;

        // _-2Pf[90] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.PlaceObjectMessageComposer;
        public const int PlaceObjectMessageEvent = 90;

        // _-2Pf[91] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.MoveWallItemMessageComposer;
        public const int MoveWallItemMessageEvent = 91;

        // _-2Pf[3002] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.PlacePetMessageComposer;
        public const int PlacePetMessageEvent = 3002;

        // _-2Pf[3003] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.RemovePetFromFlatMessageComposer;
        public const int RemovePetFromFlatMessageEvent = 3003;

        // _-2Pf[3103] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.ViralTeaserFoundMessageComposer;
        public const int ViralTeaserFoundMessageEvent = 3103;

        // _-2Pf[3110] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetMOTDMessageComposer;
        public const int GetMOTDMessageEvent = 3110;

        // _-2Pf[3105] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetUserNotificationsMessageComposer;
        public const int GetUserNotificationsMessageEvent = 3105;

        // _-2Pf[211] =
        // com.sulake.habbo.communication.messages.outgoing.room.session.ChangeQueueMessageComposer;
        public const int ChangeQueueMessageEvent = 211;

        // _-2Pf[341] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.RoomDimmerGetPresetsMessageComposer;
        public const int RoomDimmerGetPresetsMessageEvent = 341;

        // _-2Pf[342] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.RoomDimmerSavePresetMessageComposer;
        public const int RoomDimmerSavePresetMessageEvent = 342;

        // _-2Pf[343] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.RoomDimmerChangeStateMessageComposer;
        public const int RoomDimmerChangeStateMessageEvent = 343;

        // _-2Pf[393] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.UseWallItemMessageComposer;
        public const int UseWallItemMessageEvent = 393;

        // _-2Pf[83] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.GetItemDataMessageComposer;
        public const int GetItemDataMessageEvent = 83;

        // _-2Pf[84] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.SetItemDataMessageComposer;
        public const int SetItemDataMessageEvent = 84;

        // _-2Pf[85] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.RemoveItemMessageComposer;
        public const int RemoveItemMessageEvent = 85;

        // _-2Pf[3104] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.ViralFurniStatusMessageComposer;
        public const int ViralFurniStatusMessageEvent = 3104;

        // _-2Pf[3001] =
        // com.sulake.habbo.communication.messages.outgoing.room.pets.GetPetInfoMessageComposer;
        public const int GetPetInfoMessageEvent = 3001;

        // _-2Pf[215] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.GetFurnitureAliasesMessageComposer;
        public const int GetFurnitureAliasesMessageEvent = 215;

        // _-2Pf[53] =
        // com.sulake.habbo.communication.messages.outgoing.room.session.QuitMessageComposer;
        public const int QuitMessageEvent = 53;

        // _-2Pf[3254] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.PlacePostItMessageComposer;
        public const int PlacePostItMessageEvent = 3254;

        // _-2Pf[3255] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.AddSpamWallPostItMessageComposer;
        public const int AddSpamWallPostItMessageEvent = 3255;

        // _-2Pf[93] =
        // com.sulake.habbo.communication.messages.outgoing.room.avatar.DanceMessageComposer;
        public const int DanceMessageEvent = 93;

        // _-2Pf[94] =
        // com.sulake.habbo.communication.messages.outgoing.room.avatar.WaveMessageComposer;
        public const int WaveMessageEvent = 94;

        // _-2Pf[79] =
        // com.sulake.habbo.communication.messages.outgoing.room.avatar.LookToMessageComposer;
        public const int LookToMessageEvent = 79;

        // _-2Pf[76] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.ThrowDiceMessageComposer;
        public const int ThrowDiceMessageEvent = 76;

        // _-2Pf[77] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.DiceOffMessageComposer;
        public const int DiceOffMessageEvent = 77;

        // _-2Pf[78] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.PresentOpenMessageComposer;
        public const int PresentOpenMessageEvent = 78;

        // _-2Pf[183] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.CreditFurniRedeemMessageComposer;
        public const int CreditFurniRedeemMessageEvent = 183;

        // _-2Pf[232] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.EnterOneWayDoorMessageComposer;
        public const int EnterOneWayDoorMessageEvent = 232;

        // _-2Pf[247] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.SpinWheelOfFortuneMessageComposer;
        public const int SpinWheelOfFortuneMessageEvent = 247;

        // _-2Pf[314] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.SetRandomStateMessageComposer;
        public const int SetRandomStateMessageEvent = 314;

        // _-2Pf[480] =
        // com.sulake.habbo.communication.messages.outgoing.room.engine.SetClothingChangeDataMessageComposer;
        public const int SetClothingChangeDataMessageEvent = 480;

        // _-2Pf[3100] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.QuestVendingWallItemMessageComposer;
        public const int QuestVendingWallItemMessageEvent = 3100;

        // _-2Pf[3006] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.OpenPetPackageMessageComposer;
        public const int OpenPetPackageMessageEvent = 3006;

        // _-2Pf[3252] =
        // com.sulake.habbo.communication.messages.outgoing.room.furniture.OpenWelcomeGiftComposer;
        public const int OpenWelcomeGiftEvent = 3252;

        // _-2Pf[95] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.KickUserMessageComposer;
        public const int KickUserMessageEvent = 95;

        // _-2Pf[96] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.AssignRightsMessageComposer;
        public const int AssignRightsMessageEvent = 96;

        // _-2Pf[97] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.RemoveRightsMessageComposer;
        public const int RemoveRightsMessageEvent = 97;

        // _-2Pf[98] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.LetUserInMessageComposer;
        public const int LetUserInMessageEvent = 98;

        // _-2Pf[155] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.RemoveAllRightsMessageComposer;
        public const int RemoveAllRightsMessageEvent = 155;

        // _-2Pf[320] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.BanUserMessageComposer;
        public const int BanUserMessageEvent = 320;

        // _-2Pf[440] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.CallGuideBotMessageComposer;
        public const int CallGuideBotMessageEvent = 440;

        // _-2Pf[441] =
        // com.sulake.habbo.communication.messages.outgoing.room.action.KickBotMessageComposer;
        public const int KickBotMessageEvent = 441;

        // _-2Pf[263] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetUserTagsMessageComposer;
        public const int GetUserTagsMessageEvent = 263;

        // _-2Pf[159] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetSelectedBadgesMessageComposer;
        public const int GetSelectedBadgesMessageEvent = 159;

        // _-2Pf[230] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetHabboGroupBadgesMessageComposer;
        public const int GetHabboGroupBadgesMessageEvent = 230;

        // _-2Pf[231] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetHabboGroupDetailsMessageComposer;
        public const int GetHabboGroupDetailsMessageEvent = 231;

        // _-2Pf[319] =
        // com.sulake.habbo.communication.messages.outgoing.users.IgnoreUserMessageComposer;
        public const int IgnoreUserMessageEvent = 319;

        // _-2Pf[321] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetIgnoredUsersMessageComposer;
        public const int GetIgnoredUsersMessageEvent = 321;

        // _-2Pf[322] =
        // com.sulake.habbo.communication.messages.outgoing.users.UnignoreUserMessageComposer;
        public const int UnignoreUserMessageEvent = 322;

        // _-2Pf[371] =
        // com.sulake.habbo.communication.messages.outgoing.users.RespectUserMessageComposer;
        public const int RespectUserMessageEvent = 371;

        // _-2Pf[3005] =
        // com.sulake.habbo.communication.messages.outgoing.room.pets.RespectPetMessageComposer;
        public const int RespectPetMessageEvent = 3005;

        // _-2Pf[450] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.PickIssuesMessageComposer;
        public const int PickIssuesMessageEvent = 450;

        // _-2Pf[451] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.ReleaseIssuesMessageComposer;
        public const int ReleaseIssuesMessageEvent = 451;

        // _-2Pf[452] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.CloseIssuesMessageComposer;
        public const int CloseIssuesMessageEvent = 452;

        // _-2Pf[454] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.GetModeratorUserInfoMessageComposer;
        public const int GetModeratorUserInfoMessageEvent = 454;

        // _-2Pf[455] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.GetUserChatlogMessageComposer;
        public const int GetUserChatlogMessageEvent = 455;

        // _-2Pf[456] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.GetRoomChatlogMessageComposer;
        public const int GetRoomChatlogMessageEvent = 456;

        // _-2Pf[457] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.GetCfhChatlogMessageComposer;
        public const int GetCfhChatlogMessageEvent = 457;

        // _-2Pf[458] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.GetRoomVisitsMessageComposer;
        public const int GetRoomVisitsMessageEvent = 458;

        // _-2Pf[459] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.GetModeratorRoomInfoMessageComposer;
        public const int GetModeratorRoomInfoMessageEvent = 459;

        // _-2Pf[460] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.ModerateRoomMessageComposer;
        public const int ModerateRoomMessageEvent = 460;

        // _-2Pf[461] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.ModAlertMessageComposer;
        public const int ModAlertMessageEvent = 461;

        // _-2Pf[462] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.ModMessageMessageComposer;
        public const int ModMessageMessageEvent = 462;

        // _-2Pf[463] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.ModKickMessageComposer;
        public const int ModKickMessageEvent = 463;

        // _-2Pf[464] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.ModBanMessageComposer;
        public const int ModBanMessageEvent = 464;

        // _-2Pf[200] =
        // com.sulake.habbo.communication.messages.outgoing.moderator.ModeratorActionMessageComposer;
        public const int ModeratorActionMessageEvent = 200;

        // _-2Pf[453] =
        // com.sulake.habbo.communication.messages.outgoing.help.CallForHelpMessageComposer;
        public const int CallForHelpMessageEvent = 453;

        // _-2Pf[237] =
        // com.sulake.habbo.communication.messages.outgoing.help.GetPendingCallsForHelpMessageComposer;
        public const int GetPendingCallsForHelpMessageEvent = 237;

        // _-2Pf[238] =
        // com.sulake.habbo.communication.messages.outgoing.help.DeletePendingCallsForHelpMessageComposer;
        public const int DeletePendingCallsForHelpMessageEvent = 238;

        // _-2Pf[416] =
        // com.sulake.habbo.communication.messages.outgoing.help.GetClientFaqsMessageComposer;
        public const int GetClientFaqsMessageEvent = 416;

        // _-2Pf[417] =
        // com.sulake.habbo.communication.messages.outgoing.help.GetFaqCategoriesMessageComposer;
        public const int GetFaqCategoriesMessageEvent = 417;

        // _-2Pf[418] =
        // com.sulake.habbo.communication.messages.outgoing.help.GetFaqTextMessageComposer;
        public const int GetFaqTextMessageEvent = 418;

        // _-2Pf[419] =
        // com.sulake.habbo.communication.messages.outgoing.help.SearchFaqsMessageComposer;
        public const int SearchFaqsMessageEvent = 419;

        // _-2Pf[420] =
        // com.sulake.habbo.communication.messages.outgoing.help.GetFaqCategoryMessageComposer;
        public const int GetFaqCategoryMessageEvent = 420;

        // _-2Pf[3253] =
        // com.sulake.habbo.communication.messages.outgoing.users.ChangeEmailComposer;
        public const int ChangeEmailEvent = 3253;

        // _-2Pf[26] =
        // com.sulake.habbo.communication.messages.outgoing.users.ScrGetUserInfoMessageComposer;
        public const int ScrGetUserInfoMessageEvent = 26;

        // _-2Pf[481] =
        // com.sulake.habbo.communication.messages.outgoing.users.GetAchievementShareIdComposer;
        public const int GetAchievementShareIdEvent = 481;

        // _-2Pf[3111] =
        // com.sulake.habbo.communication.messages.outgoing.notifications.ResetUnseenItemsComposer;
        public const int ResetUnseenItemsEvent = 3111;

        // _-2Pf[100] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.PurchaseFromCatalogComposer;
        public const int PurchaseFromCatalogEvent = 100;

        // _-2Pf[472] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.PurchaseFromCatalogAsGiftComposer;
        public const int PurchaseFromCatalogAsGiftEvent = 472;

        // _-2Pf[473] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetGiftWrappingConfigurationComposer;
        public const int GetGiftWrappingConfigurationEvent = 473;

        // _-2Pf[475] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.SelectClubGiftComposer;
        public const int SelectClubGiftEvent = 475;

        // _-2Pf[101] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetCatalogIndexComposer;
        public const int GetCatalogIndexEvent = 101;

        // _-2Pf[102] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetCatalogPageComposer;
        public const int GetCatalogPageEvent = 102;

        // _-2Pf[129] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.RedeemVoucherMessageComposer;
        public const int RedeemVoucherMessageEvent = 129;

        // _-2Pf[3030] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetIsOfferGiftableComposer;
        public const int GetIsOfferGiftableEvent = 3030;

        // _-2Pf[3031] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetClubOffersMessageComposer;
        public const int GetClubOffersMessageEvent = 3031;

        // _-2Pf[3007] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetSellablePetBreedsComposer;
        public const int GetSellablePetBreedsEvent = 3007;

        // _-2Pf[3034] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.MarkCatalogNewAdditionsPageOpenedComposer;
        public const int MarkCatalogNewAdditionsPageOpenedEvent = 3034;

        // _-2Pf[3035] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetHabboClubExtendOfferMessageComposer;
        public const int GetHabboClubExtendOfferMessageEvent = 3035;

        // _-2Pf[3036] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.PurchaseVipMembershipExtensionComposer;
        public const int PurchaseVipMembershipExtensionEvent = 3036;

        // _-2Pf[3037] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.PurchaseBasicMembershipExtensionComposer;
        public const int PurchaseBasicMembershipExtensionEvent = 3037;

        // _-2Pf[3038] =
        // com.sulake.habbo.communication.messages.outgoing.catalog.GetHabboBasicMembershipExtendOfferComposer;
        public const int GetHabboBasicMembershipExtendOfferEvent = 3038;

        // _-2Pf[412] =
        // com.sulake.habbo.communication.messages.outgoing.recycler.GetRecyclerPrizesMessageComposer;
        public const int GetRecyclerPrizesMessageEvent = 412;

        // _-2Pf[413] =
        // com.sulake.habbo.communication.messages.outgoing.recycler.GetRecyclerStatusMessageComposer;
        public const int GetRecyclerStatusMessageEvent = 413;

        // _-2Pf[414] =
        // com.sulake.habbo.communication.messages.outgoing.recycler.RecycleItemsMessageComposer;
        public const int RecycleItemsMessageEvent = 414;

        // _-2Pf[126] =
        // com.sulake.habbo.communication.messages.outgoing.advertisement.GetRoomAdMessageComposer;
        public const int GetRoomAdMessageEvent = 126;

        // _-2Pf[182] =
        // com.sulake.habbo.communication.messages.outgoing.advertisement.GetInterstitialMessageComposer;
        public const int GetInterstitialMessageEvent = 182;

        // _-2Pf[315] =
        // com.sulake.habbo.communication.messages.outgoing.tracking.LatencyPingRequestMessageComposer;
        public const int LatencyPingRequestMessageEvent = 315;

        // _-2Pf[316] =
        // com.sulake.habbo.communication.messages.outgoing.tracking.LatencyPingReportMessageComposer;
        public const int LatencyPingReportMessageEvent = 316;

        // _-2Pf[421] =
        // com.sulake.habbo.communication.messages.outgoing.tracking.PerformanceLogMessageComposer;
        public const int PerformanceLogMessageEvent = 421;

        // _-2Pf[422] =
        // com.sulake.habbo.communication.messages.outgoing.tracking.LagWarningReportMessageComposer;
        public const int LagWarningReportMessageEvent = 422;

        // _-2Pf[482] =
        // com.sulake.habbo.communication.messages.outgoing.tracking.EventLogMessageComposer;
        public const int EventLogMessageEvent = 482;

        // _-2Pf[112] =
        // com.sulake.habbo.communication.messages.outgoing.poll.VoteAnswerMessageComposer;
        public const int VoteAnswerMessageEvent = 112;

        // _-2Pf[235] =
        // com.sulake.habbo.communication.messages.outgoing.poll.PollRejectComposer;
        public const int PollRejectEvent = 235;

        // _-2Pf[234] =
        // com.sulake.habbo.communication.messages.outgoing.poll.PollStartComposer;
        public const int PollStartEvent = 234;

        // _-2Pf[236] =
        // com.sulake.habbo.communication.messages.outgoing.poll.PollAnswerComposer;
        public const int PollAnswerEvent = 236;

        // _-2Pf[108] =
        // com.sulake.habbo.communication.messages.outgoing.room.publicroom.ExitLockerRoomMessageComposer;
        public const int ExitLockerRoomMessageEvent = 108;

        // _-2Pf[111] =
        // com.sulake.habbo.communication.messages.outgoing.room.publicroom.ChangeRoomMessageComposer;
        public const int ChangeRoomMessageEvent = 111;

        // _-2Pf[113] =
        // com.sulake.habbo.communication.messages.outgoing.room.publicroom.TryBusMessageComposer;
        public const int TryBusMessageEvent = 113;

        // _-2Pf[42] =
        // com.sulake.habbo.communication.messages.outgoing.users.ApproveNameMessageComposer;
        public const int ApproveNameMessageEvent = 42;

        // _-2Pf[245] =
        // com.sulake.habbo.communication.messages.outgoing.sound.GetSoundMachinePlayListMessageComposer;
        public const int GetSoundMachinePlayListMessageEvent = 245;

        // _-2Pf[249] =
        // com.sulake.habbo.communication.messages.outgoing.sound.GetNowPlayingMessageComposer;
        public const int GetNowPlayingMessageEvent = 249;

        // _-2Pf[221] =
        // com.sulake.habbo.communication.messages.outgoing.sound.GetSongInfoMessageComposer;
        public const int GetSongInfoMessageEvent = 221;

        // _-2Pf[228] =
        // com.sulake.habbo.communication.messages.outgoing.sound.GetSoundSettingsComposer;
        public const int GetSoundSettingsEvent = 228;

        // _-2Pf[229] =
        // com.sulake.habbo.communication.messages.outgoing.sound.SetSoundSettingsComposer;
        public const int SetSoundSettingsEvent = 229;

        // _-2Pf[0xFF] =
        // com.sulake.habbo.communication.messages.outgoing.sound.AddJukeboxDiskComposer;
        public const int AddJukeboxDiskEvent = 0xFF;

        // _-2Pf[0x0100] =
        // com.sulake.habbo.communication.messages.outgoing.sound.RemoveJukeboxDiskComposer;
        public const int RemoveJukeboxDiskEvent = 0x0100;

        // _-2Pf[258] =
        // com.sulake.habbo.communication.messages.outgoing.sound.GetJukeboxPlayListMessageComposer;
        public const int GetJukeboxPlayListMessageEvent = 258;

        // _-2Pf[259] =
        // com.sulake.habbo.communication.messages.outgoing.sound.GetUserSongDisksMessageComposer;
        public const int GetUserSongDisksMessageEvent = 259;

        // _-2Pf[375] =
        // com.sulake.habbo.communication.messages.outgoing.avatar.GetWardrobeMessageComposer;
        public const int GetWardrobeMessageEvent = 375;

        // _-2Pf[376] =
        // com.sulake.habbo.communication.messages.outgoing.avatar.SaveWardrobeOutfitMessageComposer;
        public const int SaveWardrobeOutfitMessageEvent = 376;

        // _-2Pf[470] =
        // com.sulake.habbo.communication.messages.outgoing.avatar.ChangeUserNameMessageComposer;
        public const int ChangeUserNameMessageEvent = 470;

        // _-2Pf[471] =
        // com.sulake.habbo.communication.messages.outgoing.avatar.CheckUserNameMessageComposer;
        public const int CheckUserNameMessageEvent = 471;

        // _-2Pf[44] =
        // com.sulake.habbo.communication.messages.outgoing.register.UpdateFigureDataMessageComposer;
        public const int UpdateFigureDataMessageEvent = 44;

        // _-2Pf[3050] =
        // com.sulake.habbo.communication.messages.outgoing.userdefinedroomevents.UpdateTriggerMessageComposer;
        public const int UpdateTriggerMessageEvent = 3050;

        // _-2Pf[3051] =
        // com.sulake.habbo.communication.messages.outgoing.userdefinedroomevents.UpdateActionMessageComposer;
        public const int UpdateActionMessageEvent = 3051;

        // _-2Pf[3052] =
        // com.sulake.habbo.communication.messages.outgoing.userdefinedroomevents.UpdateConditionMessageComposer;
        public const int UpdateConditionMessageEvent = 3052;

        // _-2Pf[3053] =
        // com.sulake.habbo.communication.messages.outgoing.userdefinedroomevents.OpenMessageComposer;
        public const int OpenMessageEvent = 3053;

        // _-2Pf[3054] =
        // com.sulake.habbo.communication.messages.outgoing.userdefinedroomevents.ApplySnapshotMessageComposer;
        public const int ApplySnapshotMessageEvent = 3054;

        // _-2Pf[3101] =
        // com.sulake.habbo.communication.messages.outgoing.quest.GetQuestsMessageComposer;
        public const int GetQuestsMessageEvent = 3101;

        // _-2Pf[3102] =
        // com.sulake.habbo.communication.messages.outgoing.quest.AcceptQuestMessageComposer;
        public const int AcceptQuestMessageEvent = 3102;

        // _-2Pf[3106] =
        // com.sulake.habbo.communication.messages.outgoing.quest.RejectQuestMessageComposer;
        public const int RejectQuestMessageEvent = 3106;

        // _-2Pf[3107] =
        // com.sulake.habbo.communication.messages.outgoing.quest.OpenQuestTrackerMessageComposer;
        public const int OpenQuestTrackerMessageEvent = 3107;

        // _-2Pf[3108] =
        // com.sulake.habbo.communication.messages.outgoing.quest.StartCampaignMessageComposer;
        public const int StartCampaignMessageEvent = 3108;

        // _-2Pf[3210] =
        // com.sulake.habbo.communication.messages.outgoing.quest.FriendRequestQuestCompleteMessageComposer;
        public const int FriendRequestQuestCompleteMessageEvent = 3210;

        // _-2Pf[3300] =
        // com.sulake.habbo.communication.messages.outgoing.facebook.FaceBookIsLoggedOnMessageComposer;
        public const int FaceBookIsLoggedOnMessageEvent = 3300;

        // _-2Pf[3301] =
        // com.sulake.habbo.communication.messages.outgoing.facebook.FaceBookIsLoggedOffMessageComposer;
        public const int FaceBookIsLoggedOffMessageEvent = 3301;

        // _-2Pf[3311] =
        // com.sulake.habbo.communication.messages.outgoing.facebook.FaceBookInitiateAddFriendsMessageComposer;
        public const int FaceBookInitiateAddFriendsMessageEvent = 3311;
    }
}
