using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class ModAlertMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            uint UserId = Packet.PopWiredUInt();
            string Message = Packet.PopFixedString();

            UberEnvironment.GetGame().GetModerationTool().AlertUser(Session, UserId, Message, true);
        }
    }
}
