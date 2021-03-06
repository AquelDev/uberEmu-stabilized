using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopWiredInt32();
            uint ItemId = Packet.PopWiredUInt();
            string ExtraData = Packet.PopFixedString();

            UberEnvironment.GetGame().GetCatalog().HandlePurchase(Session, PageId, ItemId, ExtraData, false, "", "");
        }
    }
}
