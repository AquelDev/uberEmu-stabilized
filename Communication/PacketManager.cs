using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.Communication.Incoming.Catalog;
using Uber.Communication.Outgoing;

namespace Uber.Communication
{
    internal sealed class PacketManager
    {
        /// <summary>
        /// Request handlers, handled upon request
        /// </summary>
        private Dictionary<uint, IPacketEvent> RequestHandlers;

        /// <summary>
        /// Storage of names from Info Events.
        /// </summary>
        public Dictionary<string, uint> InfoEvents;

        /// <summary>
        /// Initializes Packet Manager
        /// </summary>
        public PacketManager()
        {
            this.RequestHandlers = new Dictionary<uint, IPacketEvent>();
            this.InfoEvents = new Dictionary<string, uint>();
            this.Handshake();
            this.Catalog();
            this.Messenger();
            this.Help();
            this.Navigator();
            this.Rooms();
            this.Users();
            foreach (var Packet in typeof(ServerPacketHeader).GetFields())
            {
                var PacketId = (uint)Packet.GetValue(0);
                var PacketName = Packet.Name;

                if (!InfoEvents.ContainsValue(PacketId))
                {
                    InfoEvents.Add(PacketName, PacketId);
                }
            }

        }

        public bool Handle(uint PacketId, out IPacketEvent Event)
        {
            if (this.RequestHandlers.ContainsKey(PacketId))
            {
                Event = this.RequestHandlers[PacketId];
                return true;
            }
            else
            {
                Event = null;
                return false;
            }
        }

        public void Handshake()
        {
        }

        public void Catalog()
        {
            this.RequestHandlers.Add(101, new GetCatalogIndexEvent());
            this.RequestHandlers.Add(102, new GetCatalogPageEvent());
            this.RequestHandlers.Add(129, new RedeemVoucherMessageEvent());
            this.RequestHandlers.Add(100, new PurchaseFromCatalogEvent());
            this.RequestHandlers.Add(472, new PurchaseFromCatalogAsGiftEvent());
            this.RequestHandlers.Add(412, new GetRecyclerPrizesMessageEvent());
            this.RequestHandlers.Add(3030, new GetIsOfferGiftableEvent());
            this.RequestHandlers.Add(3011, new GetMarketplaceConfigurationMessageEvent());
            this.RequestHandlers.Add(473, new GetGiftWrappingConfigurationEvent());
            this.RequestHandlers.Add(3012, new GetMarketplaceCanMakeOfferEvent());
            this.RequestHandlers.Add(3010, new MakeOfferMessageEvent());
            this.RequestHandlers.Add(3019, new GetOwnOffersMessageEvent());
            this.RequestHandlers.Add(3015, new CancelOfferMessageEvent());
            this.RequestHandlers.Add(3016, new RedeemOfferCreditsMessageEvent());
            this.RequestHandlers.Add(3018, new GetOffersMessageEvent());
            this.RequestHandlers.Add(3014, new BuyOfferMessageEvent());
            this.RequestHandlers.Add(42, new ApproveNameMessageEvent());
        }

        public void Messenger()
        {
        }

        public void Help()
        {

        }

        public void Navigator()
        {
        }

        public void Rooms()
        {
        }

        public void Users()
        {
        }
    }
}
