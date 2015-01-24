using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Support;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetCfhChatlogMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            SupportTicket Ticket = UberEnvironment.GetGame().GetModerationTool().GetTicket(Packet.PopWiredUInt());

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
    }
}
