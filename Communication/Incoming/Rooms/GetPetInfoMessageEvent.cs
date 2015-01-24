using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Rooms
{
    class GetPetInfoMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            uint PetId = Packet.PopWiredUInt();

            DataRow Row = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("petid", PetId);
                Row = dbClient.ReadDataRow("SELECT * FROM user_pets WHERE id = @petid LIMIT 1");
            }

            if (Row == null)
            {
                return;
            }

            Session.SendPacket(UberEnvironment.GetGame().GetCatalog().GeneratePetFromRow(Row).SerializeInfo());
        }
    }
}
