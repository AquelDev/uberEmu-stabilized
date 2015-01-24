using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.Advertisements;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetInterstitialMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            RoomAdvertisement Ad = UberEnvironment.GetGame().GetAdvertisementManager().GetRandomRoomAdvertisement();

            ServerPacket packet = new ServerPacket(258);

            if (Ad == null)
            {
                packet.AppendStringWithBreak("");
                packet.AppendStringWithBreak("");
            }
            else
            {
                packet.AppendStringWithBreak(Ad.AdImage);
                packet.AppendStringWithBreak(Ad.AdLink);

                Ad.OnView();
            }

            Session.SendPacket(packet);
        }
    }
}
