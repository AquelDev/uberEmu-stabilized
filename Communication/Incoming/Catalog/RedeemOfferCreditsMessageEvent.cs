using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Catalog
{
    class RedeemOfferCreditsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            DataTable Results = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Results = dbClient.ReadDataTable("SELECT asking_price FROM catalog_marketplace_offers WHERE user_id = '" + Session.GetHabbo().Id + "' AND state = '2'");
            }

            if (Results == null)
            {
                return;
            }

            int Profit = 0;

            foreach (DataRow Row in Results.Rows)
            {
                Profit += (int)Row["asking_price"];
            }

            if (Profit >= 1)
            {
                Session.GetHabbo().Credits += Profit;
                Session.GetHabbo().UpdateCreditsBalance(true);
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM catalog_marketplace_offers WHERE user_id = '" + Session.GetHabbo().Id + "' AND state = '2'");
            }
        }
    }
}
