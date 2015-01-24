using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetModeratorUserInfoMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            uint UserId = Packet.PopWiredUInt();

            if (UberEnvironment.GetGame().GetClientManager().GetNameById(UserId) != "Unknown User")
            {
                Session.SendPacket(UberEnvironment.GetGame().GetModerationTool().SerializeUserInfo(UserId));
            }
            else
            {
                Session.SendNotif("Could not load user info; invalid user.");
            }
        }
    }
}
