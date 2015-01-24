using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Items;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class MakeOfferMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetInventoryComponent() == null)
            {
                return;
            }

            int _price = Packet.PopWiredInt32();
            int junk = Packet.PopWiredInt32();
            uint _id = Packet.PopWiredUInt();

            UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(_id);

            if (Item == null || !Item.GetBaseItem().AllowTrade)
            {
                return;
            }

            UberEnvironment.GetGame().GetCatalog().GetMarketplace().SellItem(Session, Item.Id, _price);
        }
    }
}
