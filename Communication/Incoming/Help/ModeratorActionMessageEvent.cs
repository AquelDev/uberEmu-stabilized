using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class ModeratorActionMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int One = Packet.PopWiredInt32();
            int Two = Packet.PopWiredInt32();
            string Message = Packet.PopFixedString();

            UberEnvironment.GetGame().GetModerationTool().RoomAlert(Session.GetHabbo().CurrentRoomId, !Two.Equals(3), Message);
        }
    }
}
