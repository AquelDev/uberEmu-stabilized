using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Users.Messenger;

namespace Uber.Messages
{
    partial class GameClientMessageHandler
    {
        private void MessengerInitMessageEvent()
        {
            Session.GetHabbo().InitMessenger();
        }

        private void FriendListUpdateMessageEvent()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.SendPacket(Session.GetHabbo().GetMessenger().SerializeUpdates());
        }

        private void RemoveBuddyMessageEvent()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Requests = Request.PopWiredInt32();

            for (int i = 0; i < Requests; i++)
            {
                Session.GetHabbo().GetMessenger().DestroyFriendship(Request.PopWiredUInt());
            }
        }

        private void HabboSearchMessageEvent()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.SendPacket(Session.GetHabbo().GetMessenger().PerformSearch(Request.PopFixedString()));
        }

        private void AcceptBuddyMessageEvent()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Amount = Request.PopWiredInt32();

            for (int i = 0; i < Amount; i++)
            {
                uint RequestId = Request.PopWiredUInt();

                MessengerRequest MessRequest = Session.GetHabbo().GetMessenger().GetRequest(RequestId);

                if (MessRequest == null)
                {
                    continue;
                }

                if (MessRequest.To != Session.GetHabbo().Id)
                {
                    // not this user's request. filthy haxxor!
                    return;
                }

                if (!Session.GetHabbo().GetMessenger().FriendshipExists(MessRequest.To, MessRequest.From))
                {
                    Session.GetHabbo().GetMessenger().CreateFriendship(MessRequest.From);
                }

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
        }

        private void DeclineBuddyMessageEvent()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            // Remove all = @f I H
            // Remove specific = @f H I <reqid>

            int Mode = Request.PopWiredInt32();
            int Amount = Request.PopWiredInt32();

            if (Mode == 0 && Amount == 1)
            {
                uint RequestId = Request.PopWiredUInt();

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
            else if (Mode == 1)
            {
                Session.GetHabbo().GetMessenger().HandleAllRequests();
            }
            else { } // todo: remove breakpoint - eventually -, but leave for a while to make sure the structure is correct and this never happens
        }

        private void RequestBuddyMessageEvent()
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().RequestBuddy(Request.PopFixedString());
        }

        private void SendMsgMessageEvent()
        {
            uint userId = Request.PopWiredUInt();
            string message = UberEnvironment.FilterInjectionChars(Request.PopFixedString());

            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);
        }

        private void FollowFriendMessageEvent()
        {
            uint BuddyId = Request.PopWiredUInt();

            GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(BuddyId);

            if (Client == null || Client.GetHabbo() == null || !Client.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Client.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            // D^HjTX]X
            GetResponse().Init(286);
            GetResponse().AppendBoolean(Room.IsPublic);
            GetResponse().AppendUInt(Client.GetHabbo().CurrentRoomId);
            SendResponse();

            if (!Room.IsPublic)
            {
                PrepareRoomForUser(Room.RoomId, "");
            }
        }

        private void SendRoomInviteMessageEvent()
        {
            int count = Request.PopWiredInt32();

            List<uint> UserIds = new List<uint>();

            for (int i = 0; i < count; i++)
            {
                UserIds.Add(Request.PopWiredUInt());
            }

            string message = UberEnvironment.FilterInjectionChars(Request.PopFixedString(), true);

            ServerPacket Message = new ServerPacket(135);
            Message.AppendUInt(Session.GetHabbo().Id);
            Message.AppendStringWithBreak(message);

            foreach (uint Id in UserIds)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id, Id))
                {
                    continue;
                }

                GameClient Client = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Id);

                if (Client == null)
                {
                    return;
                }

                Client.SendPacket(Message);
            }
        }

        public void RegisterMessenger()
        {
            RequestHandlers[12] = new RequestHandler(MessengerInitMessageEvent);
            RequestHandlers[15] = new RequestHandler(FriendListUpdateMessageEvent);
            RequestHandlers[40] = new RequestHandler(RemoveBuddyMessageEvent);
            RequestHandlers[41] = new RequestHandler(HabboSearchMessageEvent);
            RequestHandlers[33] = new RequestHandler(SendMsgMessageEvent);
            RequestHandlers[37] = new RequestHandler(AcceptBuddyMessageEvent);
            RequestHandlers[38] = new RequestHandler(DeclineBuddyMessageEvent);
            RequestHandlers[39] = new RequestHandler(RequestBuddyMessageEvent);
            RequestHandlers[262] = new RequestHandler(FollowFriendMessageEvent);
            RequestHandlers[34] = new RequestHandler(SendRoomInviteMessageEvent);
        }
    }
}
