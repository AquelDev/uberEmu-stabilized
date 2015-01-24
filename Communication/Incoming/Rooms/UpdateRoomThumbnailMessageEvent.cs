using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Rooms
{
    class UpdateRoomThumbnailMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            int Junk = Packet.PopWiredInt32(); // always 3

            Dictionary<int, int> Items = new Dictionary<int, int>();

            int Background = Packet.PopWiredInt32();
            int TopLayer = Packet.PopWiredInt32();
            int AmountOfItems = Packet.PopWiredInt32();

            for (int i = 0; i < AmountOfItems; i++)
            {
                int Pos = Packet.PopWiredInt32();
                int Item = Packet.PopWiredInt32();

                if (Pos < 0 || Pos > 10)
                {
                    return;
                }

                if (Item < 1 || Item > 27)
                {
                    return;
                }

                if (Items.ContainsKey(Pos))
                {
                    return;
                }

                Items.Add(Pos, Item);
            }

            if (Background < 1 || Background > 24)
            {
                return;
            }

            if (TopLayer < 0 || TopLayer > 11)
            {
                return;
            }

            StringBuilder FormattedItems = new StringBuilder();
            int j = 0;

            foreach (KeyValuePair<int, int> Item in Items)
            {
                if (j > 0)
                {
                    FormattedItems.Append("|");
                }

                FormattedItems.Append(Item.Key + "," + Item.Value);

                j++;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE rooms SET icon_bg = '" + Background + "', icon_fg = '" + TopLayer + "', icon_items = '" + FormattedItems.ToString() + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            Room.Icon = new RoomIcon(Background, TopLayer, Items);

            ServerPacket packet = new ServerPacket(457);
            packet.AppendUInt(Room.RoomId);
            packet.AppendBoolean(true);
            Session.SendPacket(packet);

            packet = new ServerPacket(456);
            packet.AppendUInt(Room.RoomId);
            Session.SendPacket(packet);

            RoomData Data = new RoomData();
            Data.Fill(Room);

            ServerPacket _packet = new ServerPacket(454);
            _packet.AppendBoolean(false);
            Data.Serialize(_packet, false);
            _packet.AppendBoolean(false);
            _packet.AppendBoolean(false);
            Session.SendPacket(_packet);
        }
    }
}
