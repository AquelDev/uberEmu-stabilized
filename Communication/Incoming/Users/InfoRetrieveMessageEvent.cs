using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Users
{
    class InfoRetrieveMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(5);
            packet.AppendStringWithBreak(Session.GetHabbo().Id.ToString());
            packet.AppendStringWithBreak(Session.GetHabbo().Username);
            packet.AppendStringWithBreak(Session.GetHabbo().Look);
            packet.AppendStringWithBreak(Session.GetHabbo().Gender.ToUpper());
            packet.AppendStringWithBreak(Session.GetHabbo().Motto);
            packet.AppendStringWithBreak(Session.GetHabbo().RealName);
            packet.AppendInt32(0);
            packet.AppendStringWithBreak("");
            packet.AppendInt32(0);
            packet.AppendInt32(0);
            packet.AppendInt32(Session.GetHabbo().Respect);
            packet.AppendInt32(Session.GetHabbo().DailyRespectPoints); // respect to give away
            packet.AppendInt32(Session.GetHabbo().DailyPetRespectPoints);
            Session.SendPacket(packet);
        }
    }
}
