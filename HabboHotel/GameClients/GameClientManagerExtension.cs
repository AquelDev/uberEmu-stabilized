﻿using System;
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

            Dictionary<uint, GameClient>.Enumerator eClients = this.Clients.GetEnumerator();

            while (eClients.MoveNext())
            {
                GameClient Client = eClients.Current.Value;

                if (Client.GetHabbo() != null && Client.GetHabbo().Username.ToLower() == Username.ToLower())
                {
                    ToRemove.Add(Client.ClientId);
                    continue;
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
            Dictionary<uint, GameClient>.Enumerator eClients = this.Clients.GetEnumerator();

            while (eClients.MoveNext())
            {
                GameClient Client = eClients.Current.Value;

                if (Client.GetHabbo() == null)
                {
                    continue;
                }

                int newCredits = 0;

                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    newCredits = (int)dbClient.ReadDataRow("SELECT credits FROM users WHERE id = '" + Client.GetHabbo().Id + "' LIMIT 1")[0];
                }

                int oldBalance = Client.GetHabbo().Credits;

                Client.GetHabbo().Credits = newCredits;

                if (oldBalance < 3000)
                {
                    Client.GetHabbo().UpdateCreditsBalance(false);
                    //Client.SendNotif("Credits Notification" + Convert.ToChar(13) + Convert.ToChar(13) + "We have refilled your credits to the set amount.");
                }
                else if (oldBalance >= 3000)
                {
                    Client.SendNotif("Credits Notification" + Convert.ToChar(13) + Convert.ToChar(13) + "Sorry, your credit balance is too high and has not been refilled.");
                }
            }
        }

        public void CheckForAllBanConflicts()
        {
            Dictionary<GameClient, ModerationBanException> ConflictsFound = new Dictionary<GameClient, ModerationBanException>();

            Dictionary<uint, GameClient>.Enumerator eClients = this.Clients.GetEnumerator();

            while (eClients.MoveNext())
            {
                GameClient Client = eClients.Current.Value;

                try
                {
                    UberEnvironment.GetGame().GetBanManager().CheckForBanConflicts(Client);
                }

                catch (ModerationBanException e)
                {
                    ConflictsFound.Add(Client, e);
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
                Dictionary<uint, GameClient>.Enumerator eClients = this.Clients.GetEnumerator();

                while (eClients.MoveNext())
                {
                    GameClient Client = eClients.Current.Value;

                    if (Client.GetHabbo() == null || !UberEnvironment.GetGame().GetPixelManager().NeedsUpdate(Client))
                    {
                        continue;
                    }

                    UberEnvironment.GetGame().GetPixelManager().GivePixels(Client);
                }
            }

            catch (Exception e)
            {
                UberEnvironment.GetLogging().WriteLine("[GCMExt.CheckPixelUpdates]: " + e.Message);
            }
        }
    }
}