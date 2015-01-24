using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.Catalogs;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Items;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Rooms
{
    class RecycleItemsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int itemCount = Packet.PopWiredInt32();

            if (itemCount != 5)
            {
                return;
            }

            for (int i = 0; i < itemCount; i++)
            {
                UserItem Item = Session.GetHabbo().GetInventoryComponent().GetItem(Packet.PopWiredUInt());

                if (Item != null && Item.GetBaseItem().AllowRecycle)
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);
                }
                else
                {
                    return;
                }
            }

            uint newItemId = UberEnvironment.GetGame().GetCatalog().GenerateItemId();
            EcotronReward Reward = UberEnvironment.GetGame().GetCatalog().GetRandomEcotronReward();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("INSERT INTO user_items (id,user_id,base_item,extra_data) VALUES ('" + newItemId + "','" + Session.GetHabbo().Id + "','1478','" + DateTime.Now.ToLongDateString() + "')");
                dbClient.ExecuteQuery("INSERT INTO user_presents (item_id,base_id,amount,extra_data) VALUES ('" + newItemId + "','" + Reward.BaseId + "','1','')");
            }

            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

            ServerPacket packet = new ServerPacket(508);
            packet.AppendBoolean(true);
            packet.AppendUInt(newItemId);
            Session.SendPacket(packet);
        }
    }
}
