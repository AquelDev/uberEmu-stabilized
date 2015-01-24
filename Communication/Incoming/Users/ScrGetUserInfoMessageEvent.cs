using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Users
{
    class ScrGetUserInfoMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            string SubscriptionId = Packet.PopFixedString();

            ServerPacket packet = new ServerPacket(7);
            packet.AppendStringWithBreak(SubscriptionId.ToLower());

            if (Session.GetHabbo().GetSubscriptionManager().HasSubscription(SubscriptionId))
            {
                Double Expire = Session.GetHabbo().GetSubscriptionManager().GetSubscription(SubscriptionId).ExpireTime;
                Double TimeLeft = Expire - UberEnvironment.GetUnixTimestamp();
                int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
                int MonthsLeft = TotalDaysLeft / 31;

                if (MonthsLeft >= 1) MonthsLeft--;

                packet.AppendInt32(TotalDaysLeft - (MonthsLeft * 31));
                packet.AppendBoolean(true);
                packet.AppendInt32(MonthsLeft);
                packet.AppendInt32(1);
                packet.AppendInt32(1);

                if (Session.GetHabbo().HasFuse("fuse_use_vip_outfits"))
                {
                    packet.AppendInt32(2);
                }
                else
                {
                    packet.AppendInt32(1);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    packet.AppendInt32(0);
                }
            }

            Session.SendPacket(packet);
        }
    }
}
