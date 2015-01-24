using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Users.Messenger;
using Uber.HabboHotel.Support;
using Uber.HabboHotel.Pathfinding;

namespace Uber.Messages
{
    partial class GameClientMessageHandler
    {
        private void GetClientFaqsMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetHelpTool().SerializeFrontpage());
        }

        private void GetFaqCategoriesMessageEvent()
        {
            Session.SendPacket(UberEnvironment.GetGame().GetHelpTool().SerializeIndex());
        }

        private void GetFaqTextMessageEvent()
        {
            uint TopicId = Request.PopWiredUInt();

            HelpTopic Topic = UberEnvironment.GetGame().GetHelpTool().GetTopic(TopicId);

            if (Topic == null)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetHelpTool().SerializeTopic(Topic));
        }

        private void SearchFaqsMessageEvent()
        {
            string SearchQuery = Request.PopFixedString();

            if (SearchQuery.Length < 3)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetHelpTool().SerializeSearchResults(SearchQuery));
        }

        private void GetFaqCategoryMessageEvent()
        {
            uint Id = Request.PopWiredUInt();

            HelpCategory Category = UberEnvironment.GetGame().GetHelpTool().GetCategory(Id);

            if (Category == null)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetHelpTool().SerializeCategory(Category));
        }

        private void CallForHelpMessageEvent()
        {
            Boolean errorOccured = false;

            if (UberEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                errorOccured = true;
            }

            if (!errorOccured)
            {
                String Message = UberEnvironment.FilterInjectionChars(Request.PopFixedString());

                int Junk = Request.PopWiredInt32();
                int Type = Request.PopWiredInt32();
                uint ReportedUser = Request.PopWiredUInt();

                UberEnvironment.GetGame().GetModerationTool().SendNewTicket(Session, Type, ReportedUser, Message);
            }

            GetResponse().Init(321);
            GetResponse().AppendBoolean(errorOccured);
            SendResponse();
        }

        private void DeletePendingCallsForHelpMessageEvent()
        {
            if (!UberEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                return;
            }

            UberEnvironment.GetGame().GetModerationTool().DeletePendingTicketForUser(Session.GetHabbo().Id);

            GetResponse().Init(320);
            SendResponse();
        }

        private void GetModeratorUserInfoMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();

            if (UberEnvironment.GetGame().GetClientManager().GetNameById(UserId) != "Unknown User")
            {
                Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeUserInfo(UserId));
            }
            else
            {
                Session.SendNotif("Could not load user info; invalid user.");
            }
        }

        private void GetUserChatlogMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeUserChatlog(Request.PopWiredUInt()));
        }

        private void GetRoomChatlogMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            int Junk = Request.PopWiredInt32();
            uint RoomId = Request.PopWiredUInt();

            if (UberEnvironment.GetGame().GetRoomManager().GetRoom(RoomId) != null)
            {
                Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeRoomChatlog(RoomId));
            }
        }

        private void GetModeratorRoomInfoMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint RoomId = Request.PopWiredUInt();
            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(RoomId);

            Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeRoomTool(Data));
        }

        private void PickIssuesMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Junk = Request.PopWiredInt32();
            uint TicketId = Request.PopWiredUInt();
            UberEnvironment.GetGame().GetModerationTool().PickTicket(Session, TicketId);
        }

        private void ReleaseIssuesMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int amount = Request.PopWiredInt32();

            for (int i = 0; i < amount; i++)
            {
                uint TicketId = Request.PopWiredUInt();

                UberEnvironment.GetGame().GetModerationTool().ReleaseTicket(Session, TicketId);
            }
        }

        private void CloseIssuesMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Result = Request.PopWiredInt32(); // result, 1 = useless, 2 = abusive, 3 = resolved
            int Junk = Request.PopWiredInt32(); // ? 
            uint TicketId = Request.PopWiredUInt(); // id

            UberEnvironment.GetGame().GetModerationTool().CloseTicket(Session, TicketId, Result);
        }

        private void GetCfhChatlogMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            SupportTicket Ticket = UberEnvironment.GetGame().GetModerationTool().GetTicket(Request.PopWiredUInt());

            if (Ticket == null)
            {
                return;
            }

            RoomData Data = UberEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Ticket.RoomId);

            if (Data == null)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeTicketChatlog(Ticket, Data, Ticket.Timestamp));
        }

        private void GetRoomVisitsMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();

            Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeRoomVisits(UserId));
        }

        private void ModeratorActionMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int One = Request.PopWiredInt32();
            int Two = Request.PopWiredInt32();
            String Message = Request.PopFixedString();

            UberEnvironment.GetGame().GetModerationTool().RoomAlert(Session.GetHabbo().CurrentRoomId, !Two.Equals(3), Message);
        }

        private void ModerateRoomMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint RoomId = Request.PopWiredUInt();
            Boolean ActOne = Request.PopWiredBoolean(); // set room lock to doorbell
            Boolean ActTwo = Request.PopWiredBoolean(); // set room to inappropiate
            Boolean ActThree = Request.PopWiredBoolean(); // kick all users

            UberEnvironment.GetGame().GetModerationTool().PerformRoomAction(Session, RoomId, ActThree, ActOne, ActTwo);
        }

        private void ModAlertMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();

            UberEnvironment.GetGame().GetModerationTool().AlertUser(Session, UserId, Message, true);
        }

        private void ModMessageMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();

            UberEnvironment.GetGame().GetModerationTool().AlertUser(Session, UserId, Message, false);
        }

        private void ModKickMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_kick"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();

            UberEnvironment.GetGame().GetModerationTool().KickUser(Session, UserId, Message, false);
        }

        private void ModBanMessageEvent()
        {
            if (!Session.GetHabbo().HasFuse("fuse_ban"))
            {
                return;
            }

            uint UserId = Request.PopWiredUInt();
            String Message = Request.PopFixedString();
            int Length = Request.PopWiredInt32() * 3600;

            UberEnvironment.GetGame().GetModerationTool().BanUser(Session, UserId, Length, Message);
        }

        private void CallGuideBotMessageEvent()
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }
            foreach (var _user in Room.UserList)
            {
                if (_user.IsBot && _user.BotData.AiType == "guide")
                {
                    ServerPacket packet = new ServerPacket(33);
                    packet.AppendInt32(4009);
                    Session.SendPacket(packet);
                }
            }

            if (Session.GetHabbo().CalledGuideBot)
            {
                ServerPacket packet = new ServerPacket(33);
                packet.AppendInt32(4010);
                Session.SendPacket(packet);
                return;
            }

            RoomUser NewUser = Room.DeployBot(UberEnvironment.GetGame().GetBotManager().GetBot(55));
            NewUser.SetPos(Room.Model.DoorX, Room.Model.DoorY, Room.Model.DoorZ);
            NewUser.UpdateNeeded = true;

            RoomUser RoomOwner = Room.GetRoomUserByHabbo(Room.Owner);

            if (RoomOwner != null)
            {
                NewUser.MoveTo(RoomOwner.Coordinate);
                NewUser.SetRot(Rotation.Calculate(NewUser.X, NewUser.Y, RoomOwner.X, RoomOwner.Y));
            }

            UberEnvironment.GetGame().GetAchievementManager().UnlockAchievement(Session, 6, 1);
            Session.GetHabbo().CalledGuideBot = true;
        }

        public void RegisterHelp()
        {
            RequestHandlers[200] = new RequestHandler(ModeratorActionMessageEvent);
            RequestHandlers[238] = new RequestHandler(DeletePendingCallsForHelpMessageEvent);
            RequestHandlers[416] = new RequestHandler(GetClientFaqsMessageEvent);
            RequestHandlers[417] = new RequestHandler(GetFaqCategoriesMessageEvent);
            RequestHandlers[418] = new RequestHandler(GetFaqTextMessageEvent);
            RequestHandlers[419] = new RequestHandler(SearchFaqsMessageEvent);
            RequestHandlers[420] = new RequestHandler(GetFaqCategoryMessageEvent);
            RequestHandlers[440] = new RequestHandler(CallGuideBotMessageEvent);
            RequestHandlers[450] = new RequestHandler(PickIssuesMessageEvent);
            RequestHandlers[451] = new RequestHandler(ReleaseIssuesMessageEvent);
            RequestHandlers[452] = new RequestHandler(CloseIssuesMessageEvent);
            RequestHandlers[453] = new RequestHandler(CallForHelpMessageEvent);
            RequestHandlers[454] = new RequestHandler(GetModeratorUserInfoMessageEvent);
            RequestHandlers[455] = new RequestHandler(GetUserChatlogMessageEvent);
            RequestHandlers[456] = new RequestHandler(GetRoomChatlogMessageEvent);
            RequestHandlers[457] = new RequestHandler(GetCfhChatlogMessageEvent);
            RequestHandlers[458] = new RequestHandler(GetRoomVisitsMessageEvent);
            RequestHandlers[459] = new RequestHandler(GetModeratorRoomInfoMessageEvent);
            RequestHandlers[460] = new RequestHandler(ModerateRoomMessageEvent);
            RequestHandlers[461] = new RequestHandler(ModAlertMessageEvent);
            RequestHandlers[462] = new RequestHandler(ModMessageMessageEvent);
            RequestHandlers[463] = new RequestHandler(ModKickMessageEvent);
            RequestHandlers[464] = new RequestHandler(ModBanMessageEvent);
        }
    }
}