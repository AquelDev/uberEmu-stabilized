using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class ModBanMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_ban"))
            {
                return;
            }

            uint UserId = Packet.PopWiredUInt();
            string Message = Packet.PopFixedString();
            int Length = Packet.PopWiredInt32() * 3600;

            UberEnvironment.GetGame().GetModerationTool().BanUser(Session, UserId, Length, Message);
        }
    }
}
