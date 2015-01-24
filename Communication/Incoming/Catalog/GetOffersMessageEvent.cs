using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class GetOffersMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            int _min = Packet.PopWiredInt32();
            int _max = Packet.PopWiredInt32();
            string _query = Packet.PopFixedString();
            int _filter = Packet.PopWiredInt32();
            Session.SendPacket(UberEnvironment.GetGame().GetCatalog().GetMarketplace().SerializeOffers(_min, _max, _query, _filter));
        }
    }
}
