using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Users.Badges;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Users
{
    class SetActivatedBadgesEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetBadgeComponent().ResetSlots();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE user_badges SET badge_slot = '0' WHERE user_id = '" + Session.GetHabbo().Id + "'");
            }

            while (Packet.RemainingLength > 0)
            {
                int Slot = Packet.PopWiredInt32();
                string Badge = Packet.PopFixedString();

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
    }
}
