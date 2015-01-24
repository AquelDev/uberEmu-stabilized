using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Misc;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Users
{
    class SaveWardrobeOutfitMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint SlotId = Packet.PopWiredUInt();

            string Look = Packet.PopFixedString();
            string Gender = Packet.PopFixedString();

            if (!AntiMutant.ValidateLook(Look, Gender))
            {
                return;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("userid", Session.GetHabbo().Id);
                dbClient.AddParamWithValue("slotid", SlotId);
                dbClient.AddParamWithValue("look", Look);
                dbClient.AddParamWithValue("gender", Gender.ToUpper());

                if (dbClient.ReadDataRow("SELECT null FROM user_wardrobe WHERE user_id = @userid AND slot_id = @slotid LIMIT 1") != null)
                {
                    dbClient.ExecuteQuery("UPDATE user_wardrobe SET look = @look, gender = @gender WHERE user_id = @userid AND slot_id = @slotid LIMIT 1");
                }
                else
                {
                    dbClient.ExecuteQuery("INSERT INTO user_wardrobe (user_id,slot_id,look,gender) VALUES (@userid,@slotid,@look,@gender)");
                }
            }
        }
    }
}
