using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Items;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Catalog
{
    class BuyOfferMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint ItemId = Packet.PopWiredUInt();
            DataRow Row = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Row = dbClient.ReadDataRow("SELECT * FROM catalog_marketplace_offers WHERE offer_id = '" + ItemId + "' LIMIT 1");
            }

            if (Row == null || (string)Row["state"] != "1" || (double)Row["timestamp"] <= UberEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestamp())
            {
                Session.SendNotif("Sorry, this offer has expired.");
                return;
            }

            Item Item = UberEnvironment.GetGame().GetItemManager().GetItem((uint)Row["item_id"]);

            if (Item == null)
            {
                return;
            }

            if ((int)Row["total_price"] >= 1)
            {
                Session.GetHabbo().Credits -= (int)Row["total_price"];
                Session.GetHabbo().UpdateCreditsBalance(true);
            }

            UberEnvironment.GetGame().GetCatalog().DeliverItems(Session, Item, 1, (String)Row["extra_data"]);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE catalog_marketplace_offers SET state = '2' WHERE offer_id = '" + ItemId + "' LIMIT 1");
            }

            ServerPacket packet = new ServerPacket(67);
            packet.AppendUInt(Item.ItemId);
            packet.AppendStringWithBreak(Item.Name);
            packet.AppendInt32(0);
            packet.AppendInt32(0);
            packet.AppendInt32(1);
            packet.AppendStringWithBreak(Item.Type.ToLower());
            packet.AppendInt32(Item.SpriteId);
            packet.AppendStringWithBreak("");
            packet.AppendInt32(1);
            packet.AppendInt32(-1);
            packet.AppendStringWithBreak("");
            Session.SendPacket(packet);

            Session.SendPacket(UberEnvironment.GetGame().GetCatalog().GetMarketplace().SerializeOffers(-1, -1, "", 1));
        }
    }
}
