using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Items;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Rooms
{
    class PresentOpenMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            RoomItem Present = Room.GetItem(Packet.PopWiredUInt());

            if (Present == null)
            {
                return;
            }

            DataRow Data = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Data = dbClient.ReadDataRow("SELECT base_id,amount,extra_data FROM user_presents WHERE item_id = '" + Present.Id + "' LIMIT 1");
            }

            if (Data == null)
            {
                return;
            }

            Item BaseItem = UberEnvironment.GetGame().GetItemManager().GetItem((uint)Data["base_id"]);

            if (BaseItem == null)
            {
                return;
            }

            Room.RemoveFurniture(Session, Present.Id);

            ServerPacket packet = new ServerPacket(219);
            packet.AppendUInt(Present.Id);
            Session.SendPacket(packet);

            packet = new ServerPacket(129);
            packet.AppendStringWithBreak(BaseItem.Type);
            packet.AppendInt32(BaseItem.SpriteId);
            packet.AppendStringWithBreak(BaseItem.Name);
            Session.SendPacket(packet);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM user_presents WHERE item_id = '" + Present.Id + "' LIMIT 1");
            }

            UberEnvironment.GetGame().GetCatalog().DeliverItems(Session, BaseItem, (int)Data["amount"], (string)Data["extra_data"]);
        }
    }
}
