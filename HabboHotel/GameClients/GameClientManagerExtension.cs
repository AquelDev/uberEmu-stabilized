using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Uber.Storage;
using Uber.HabboHotel.Users.Authenticator;
using Uber.HabboHotel.Support;

namespace Uber.HabboHotel.GameClients
{
    partial class GameClientManager
    {
        public void LogClonesOut(string Username)
        {
            List<uint> ToRemove = new List<uint>();
            foreach (var _client in Clients)
            {
                if (_client.Value.GetHabbo() != null && _client.Value.GetHabbo().Username.ToLower() == Username.ToLower())
                {
                    ToRemove.Add(_client.Value.ClientId);
                }
            }

            for (int i = 0; i < ToRemove.Count; i++)
            {
                this.Clients[ToRemove[i]].Disconnect();
            }
        }

        public string GetNameById(uint Id)
        {
            GameClient Cl = GetClientByHabbo(Id);

            if (Cl != null)
            {
                return Cl.GetHabbo().Username;
            }

            DataRow Row = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                Row = dbClient.ReadDataRow("SELECT username FROM users WHERE id = '" + Id + "' LIMIT 1");
            }

            if (Row == null)
            {
                return "Unknown User";
            }

            return (string)Row[0];
        }

        public void DeployHotelCreditsUpdate()
        {
            foreach (var _client in Clients)
            {
                if (_client.Value.GetHabbo() == null)
                {
                    continue;
                }

                int newCredits = 0;

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    newCredits = (int)dbClient.ReadDataRow("SELECT credits FROM users WHERE id = '" + _client.Value.GetHabbo().Id + "' LIMIT 1")[0];
                }

                int oldBalance = _client.Value.GetHabbo().Credits;

                _client.Value.GetHabbo().Credits = newCredits;

                if (oldBalance < 3000)
                {
                    _client.Value.GetHabbo().UpdateCreditsBalance(false);
                }
                else if (oldBalance >= 3000)
                {
                    _client.Value.SendNotif("Credits Notification" + Convert.ToChar(13) + Convert.ToChar(13) + "Sorry, your credit balance is too high and has not been refilled.");
                }
            }
        }

        public void CheckForAllBanConflicts()
        {
            Dictionary<GameClient, ModerationBanException> ConflictsFound = new Dictionary<GameClient, ModerationBanException>();
            foreach (var _client in Clients)
            {
                try
                {
                    UberEnvironment.GetGame().GetBanManager().CheckForBanConflicts(_client.Value);
                }
                catch (ModerationBanException e)
                {
                    ConflictsFound.Add(_client.Value, e);
                }
            }

            foreach (KeyValuePair<GameClient, ModerationBanException> Data in ConflictsFound)
            {
                Data.Key.SendBanMessage(Data.Value.Message);
                Data.Key.Disconnect();
            }
        }

        public void CheckPixelUpdates()
        {
            try
            {
                foreach (var _client in Clients)
                {
                    if (_client.Value.GetHabbo() == null || !UberEnvironment.GetGame().GetPixelManager().NeedsUpdate(_client.Value))
                    {
                        continue;
                    }
                    UberEnvironment.GetGame().GetPixelManager().GivePixels(_client.Value);
                }
            }
            catch (Exception e)
            {
                UberEnvironment.GetLogging().WriteLine("[GCMExt.CheckPixelUpdates]: " + e.Message);
            }
        }
    }
}