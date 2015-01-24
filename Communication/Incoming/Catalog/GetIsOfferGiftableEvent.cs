using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.Catalogs;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class GetIsOfferGiftableEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint Id = Packet.PopWiredUInt();

            CatalogItem Item = UberEnvironment.GetGame().GetCatalog().FindItem(Id);

            if (Item == null)
            {
                return;
            }

            ServerPacket packet = new ServerPacket(622);
            packet.AppendUInt(Item.Id);
            packet.AppendBoolean(Item.GetBaseItem().AllowGift);
            Session.SendPacket(packet);
        }
    }
}
