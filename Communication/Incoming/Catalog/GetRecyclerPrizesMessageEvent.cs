using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.Catalogs;
using Uber.HabboHotel.GameClients;
using Uber.Messages;

namespace Uber.Communication.Incoming.Catalog
{
    class GetRecyclerPrizesMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(506);
            packet.AppendInt32(5);

            for (uint i = 5; i >= 1; i--)
            {
                packet.AppendUInt(i);

                if (i <= 1)
                {
                    packet.AppendInt32(0);
                }
                else if (i == 2)
                {
                    packet.AppendInt32(4);
                }
                else if (i == 3)
                {
                    packet.AppendInt32(40);
                }
                else if (i == 4)
                {
                    packet.AppendInt32(200);
                }
                else if (i >= 5)
                {
                    packet.AppendInt32(2000);
                }

                List<EcotronReward> Rewards = UberEnvironment.GetGame().GetCatalog().GetEcotronRewardsForLevel(i);

                packet.AppendInt32(Rewards.Count);

                foreach (EcotronReward Reward in Rewards)
                {
                    packet.AppendStringWithBreak(Reward.GetBaseItem().Type.ToLower());
                    packet.AppendUInt(Reward.DisplayId);
                }
            }

            Session.SendPacket(packet);
        }
    }
}
