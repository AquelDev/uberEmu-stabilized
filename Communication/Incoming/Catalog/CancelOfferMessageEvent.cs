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
    class CancelOfferMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint _id = Packet.PopWiredUInt();
            DataRow Row = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Row = dbClient.ReadDataRow("SELECT * FROM catalog_marketplace_offers WHERE offer_id = '" + _id + "' LIMIT 1");
            }

            if (Row == null || (uint)Row["user_id"] != Session.GetHabbo().Id || (string)Row["state"] != "1")
            {
                return;
            }

            Item Item = UberEnvironment.GetGame().GetItemManager().GetItem((uint)Row["item_id"]);

            if (Item == null)
            {
                return;
            }

            UberEnvironment.GetGame().GetCatalog().DeliverItems(Session, Item, 1, (String)Row["extra_data"]);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM catalog_marketplace_offers WHERE offer_id = '" + _id + "' LIMIT 1");
            }

            ServerPacket packet = new ServerPacket(614);
            packet.AppendUInt((uint)Row["offer_id"]);
            packet.AppendBoolean(true);
            Session.SendPacket(packet);
        }
    }
}
