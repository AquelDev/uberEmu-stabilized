using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Users
{
    class GetWardrobeMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket packet = new ServerPacket(267);
            packet.AppendBoolean(Session.GetHabbo().HasFuse("fuse_use_wardrobe"));

            if (Session.GetHabbo().HasFuse("fuse_use_wardrobe"))
            {
                DataTable WardrobeData = null;

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("userid", Session.GetHabbo().Id);
                    WardrobeData = dbClient.ReadDataTable("SELECT * FROM user_wardrobe WHERE user_id = @userid");
                }

                if (WardrobeData == null)
                {
                    packet.AppendInt32(0);
                }
                else
                {
                    packet.AppendInt32(WardrobeData.Rows.Count);

                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        packet.AppendUInt((uint)Row["slot_id"]);
                        packet.AppendStringWithBreak((string)Row["look"]);
                        packet.AppendStringWithBreak((string)Row["gender"]);
                    }
                }
            }
            Session.SendPacket(packet);
        }
    }
}
