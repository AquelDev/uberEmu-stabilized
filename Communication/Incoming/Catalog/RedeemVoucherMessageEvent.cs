using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class RedeemVoucherMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            UberEnvironment.GetGame().GetCatalog().GetVoucherHandler().TryRedeemVoucher(Session, Packet.PopFixedString());
        }
    }
}
