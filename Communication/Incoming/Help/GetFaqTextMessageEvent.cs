using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Support;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetFaqTextMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint TopicId = Packet.PopWiredUInt();

            HelpTopic Topic = UberEnvironment.GetGame().GetHelpTool().GetTopic(TopicId);

            if (Topic == null)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetHelpTool().SerializeTopic(Topic));
        }
    }
}
