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
        }

        private void GetFaqCategoriesMessageEvent()
        {
        }

        private void GetFaqTextMessageEvent()
        {
        }

        private void SearchFaqsMessageEvent()
        {

        }

        private void GetFaqCategoryMessageEvent()
        {

        }

        private void CallForHelpMessageEvent()
        {

        }

        private void DeletePendingCallsForHelpMessageEvent()
        {

        }

        private void GetModeratorUserInfoMessageEvent()
        {

        }

        private void GetUserChatlogMessageEvent()
        {

        }

        private void GetRoomChatlogMessageEvent()
        {

        }

        private void GetModeratorRoomInfoMessageEvent()
        {

        }

        private void PickIssuesMessageEvent()
        {

        }

        private void ReleaseIssuesMessageEvent()
        {

        }

        private void CloseIssuesMessageEvent()
        {

        }

        private void GetCfhChatlogMessageEvent()
        {

        }

        private void GetRoomVisitsMessageEvent()
        {

        }

        private void ModeratorActionMessageEvent()
        {

        }

        private void ModerateRoomMessageEvent()
        {

        }

        private void ModAlertMessageEvent()
        {

        }

        private void ModMessageMessageEvent()
        {

        }

        private void ModKickMessageEvent()
        {

        }

        private void ModBanMessageEvent()
        {

        }

        private void CallGuideBotMessageEvent()
        {

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