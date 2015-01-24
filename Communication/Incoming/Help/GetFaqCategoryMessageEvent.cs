using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Support;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class GetFaqCategoryMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint Id = Packet.PopWiredUInt();

            HelpCategory Category = UberEnvironment.GetGame().GetHelpTool().GetCategory(Id);

            if (Category == null)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetHelpTool().SerializeCategory(Category));
        }
    }
}
