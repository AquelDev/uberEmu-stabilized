using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication
{
    interface IPacketEvent
    {
        void parse(GameClient Session, ClientPacket Packet);
    }
}
