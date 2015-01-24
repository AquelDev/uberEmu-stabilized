using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class ModKickMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_kick"))
            {
                return;
            }

            uint UserId = Packet.PopWiredUInt();
            string Message = Packet.PopFixedString();

            UberEnvironment.GetGame().GetModerationTool().KickUser(Session, UserId, Message, false);
        }
    }
}
