using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class AvatarEffectSelectedEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetAvatarEffectsInventoryComponent().ApplyEffect(Packet.PopWiredInt32());
        }
    }
}
