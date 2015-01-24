using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.Catalogs;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class GetCatalogPageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            CatalogPage Page = UberEnvironment.GetGame().GetCatalog().GetPage(Packet.PopWiredInt32());

            if (Page == null || !Page.Enabled || !Page.Visible || Page.ComingSoon || Page.MinRank > Session.GetHabbo().Rank)
            {
                return;
            }

            if (Page.ClubOnly && !Session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_club"))
            {
                Session.SendNotif("This page is for Club members only!");
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetCatalog().SerializePage(Page));

            if (Page.Layout == "recycler")
            {
                ServerPacket message = new ServerPacket(507);
                message.AppendBoolean(true);
                message.AppendBoolean(false);
                Session.SendPacket(message);
            }
        }
    }
}
