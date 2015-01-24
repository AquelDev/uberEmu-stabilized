using System;
using System.Collections.Generic;
using System.Text;

using Uber.HabboHotel.Items;
using Uber.HabboHotel.Pathfinding;
using Uber.HabboHotel.GameClients;
using Uber.Messages;
using Uber.HabboHotel.Rooms;
using Uber.Storage;
using System.Data;

namespace Uber.HabboHotel.Misc
{
    class ChatCommandHandler
    {
        public static Boolean Parse(GameClient Session, string Input)
        {
            string[] Params = Input.Split(' ');

            string TargetUser = null;
            GameClient TargetClient = null;
            Room TargetRoom = null;
            RoomUser TargetRoomUser = null;

            try
            {
                switch (Params[0].ToLower())
                {
                    #region Debugging/Development
                    case "update_inventory":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                            return true;
                        }

                        return false;

                    case "update_bots":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            UberEnvironment.GetGame().GetBotManager().LoadBots();
                            return true;
                        }

                        return false;

                    case "update_catalog":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            UberEnvironment.GetGame().GetCatalog().Initialize();
                            UberEnvironment.GetGame().GetClientManager().BroadcastMessage(new ServerPacket(441));

                            return true;
                        }

                        return false;

                    case "update_help":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            UberEnvironment.GetGame().GetHelpTool().LoadCategories();
                            UberEnvironment.GetGame().GetHelpTool().LoadTopics();
                            Session.SendNotif("Reloaded help categories and topics successfully.");

                            return true;
                        }

                        return false;

                    case "update_navigator":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            UberEnvironment.GetGame().GetNavigator().Initialize();
                            Session.SendNotif("Re-initialized navigator successfully.");

                            return true;
                        }

                        return false;

                    /*case "idletime":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            TargetRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
                            TargetRoomUser = TargetRoom.GetRoomUserByHabbo(Session.GetHabbo().Id);

                            TargetRoomUser.IdleTime = 600;

                            return true;
                        }

                        return false;*/

                    case "t":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            TargetRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

                            if (TargetRoom == null)
                            {
                                return false;
                            }

                            TargetRoomUser = TargetRoom.GetRoomUserByHabbo(Session.GetHabbo().Id);

                            if (TargetRoomUser == null)
                            {
                                return false;
                            }

                            Session.SendNotif("X: " + TargetRoomUser.X + " - Y: " + TargetRoomUser.Y + " - Z: " + TargetRoomUser.Z + " - Rot: " + TargetRoomUser.RotBody);

                            return true;
                        }

                        return false;

                    case "override":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            TargetRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

                            if (TargetRoom == null)
                            {
                                return false;
                            }

                            TargetRoomUser = TargetRoom.GetRoomUserByHabbo(Session.GetHabbo().Id);

                            if (TargetRoomUser == null)
                            {
                                return false;
                            }

                            if (TargetRoomUser.AllowOverride)
                            {
                                TargetRoomUser.AllowOverride = false;
                                Session.SendNotif("Walking override disabled.");
                            }
                            else
                            {
                                TargetRoomUser.AllowOverride = true;
                                Session.SendNotif("Walking override enabled.");
                            }

                            return true;
                        }

                        return false;
                    case "moonwalkon":

                        Rotation.MoonwalkEnabled = 1;
                        Session.SendNotif("Moonwalk is now enabled.");
                        Console.WriteLine("Someone has enabled their moonwalk.");

                        break;

                    case "moonwalkoff":

                        Rotation.MoonwalkEnabled = 0;
                        Session.SendNotif("Moonwalk is now disabled..");
                        Console.WriteLine("Someone has turned their moonwalk off.");

                        break;

                    case "update_defs":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            UberEnvironment.GetGame().GetItemManager().LoadItems();
                            Session.SendNotif("Item defenitions reloaded successfully.");
                            return true;
                        }

                        return false;

                    case "whosonline":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            DataTable onlineData = new DataTable("online");
                            string message = "Users Online:\r";
                            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                            {
                                onlineData = dbClient.ReadDataTable("SELECT username FROM users WHERE online = '1';");
                            }
                            foreach (DataRow user in onlineData.Rows)
                            {
                                message = message + user["username"] + "\r";
                            }
                            Session.SendNotif(message);
                            return true;
                        }

                        return false;

                    #endregion

                    #region General Commands
                    case "pickall":

                        TargetRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

                        if (TargetRoom != null && TargetRoom.CheckRights(Session, true))
                        {
                            List<RoomItem> ToRemove = new List<RoomItem>();

                            ToRemove.AddRange(TargetRoom.Items);

                            foreach (RoomItem Item in ToRemove)
                            {
                                TargetRoom.RemoveFurniture(Session, Item.Id);
                                Session.GetHabbo().GetInventoryComponent().AddItem(Item.Id, Item.BaseItem, Item.ExtraData);
                            }

                            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                            return true;
                        }

                        return false;

                    case "empty":

                        Session.GetHabbo().GetInventoryComponent().ClearItems();

                        return true;
                    #endregion


                    #region Moderation Commands

                    case "invisible":

                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            if (Session.GetHabbo().SpectatorMode)
                            {
                                Session.GetHabbo().SpectatorMode = false;
                                Session.SendNotif("Spectator mode disabled. Reload the room to apply changes.");
                            }
                            else
                            {
                                Session.GetHabbo().SpectatorMode = true;
                                Session.SendNotif("Spectator mode enabled. Reload the room to apply changes.");
                            }

                            return true;
                        }

                        return false;

                    case "commands":
                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                            Session.SendNotif("Hello Administrator. Below are your commands~\n\n" + ":ha (message)\n:shutup <user> <message>\n:roomkick <message>\n\nMore are available.");
                        else
                            Session.SendNotif("Hello user. Below are your commands~\n\n" + ":commands - Shows this dialogue.\n:about - Tells you information about our emulator.\n:moonwalkon - Turns moonwalk on\n:moonwalkoff - Turns moonwalk off\n\nThats all for now, Enjoy!");
                        break;


                    case "ha":
                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            string Notice = Input.Substring(3);
                            ServerPacket HotelAlert = new ServerPacket(139);
                            HotelAlert.AppendStringWithBreak("Message from Hotel Management:\r\n" + Notice + "\r\n-" + Session.GetHabbo().Username);
                            UberEnvironment.GetGame().GetClientManager().BroadcastMessage(HotelAlert);

                            return true;
                        }
                        return false;

                    case "credits":
                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Params[1]);
                            if (TargetClient != null)
                            {
                                int creditsToAdd;
                                if (int.TryParse(Params[2], out creditsToAdd))
                                {
                                    TargetClient.GetHabbo().Credits = TargetClient.GetHabbo().Credits + creditsToAdd;
                                    TargetClient.GetHabbo().UpdateCreditsBalance(true);
                                    TargetClient.SendNotif(Session.GetHabbo().Username + " has awarded you " + creditsToAdd.ToString() + " credits!");
                                    Session.SendNotif("Credit balance updated successfully.");
                                    return true;
                                }
                                else
                                {
                                    Session.SendNotif("Please enter a valid amount of credits.");
                                    return false;
                                }
                            }
                            else
                            {
                                Session.SendNotif("User could not be found.");
                                return false;
                            }
                        }
                        return false;

                    case "pixels":
                        if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
                        {
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Params[1]);
                            if (TargetClient != null)
                            {
                                int creditsToAdd;
                                if (int.TryParse(Params[2], out creditsToAdd))
                                {
                                    TargetClient.GetHabbo().ActivityPoints = TargetClient.GetHabbo().ActivityPoints + creditsToAdd;
                                    TargetClient.GetHabbo().UpdateActivityPointsBalance(true);
                                    TargetClient.SendNotif(Session.GetHabbo().Username + " has awarded you " + creditsToAdd.ToString() + " pixels!");
                                    Session.SendNotif("Pixel balance updated successfully.");
                                    return true;
                                }
                                else
                                {
                                    Session.SendNotif("Please enter a valid amount of pixels.");
                                    return false;
                                }
                            }
                            else
                            {
                                Session.SendNotif("User could not be not found.");
                                return false;
                            }
                        }
                        return false;

                    case "ban":

                        if (Session.GetHabbo().HasFuse("fuse_ban"))
                        {
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Params[1]);

                            if (TargetClient == null)
                            {
                                Session.SendNotif("User not found.");
                                return true;
                            }

                            if (TargetClient.GetHabbo().Rank >= Session.GetHabbo().Rank)
                            {
                                Session.SendNotif("You are not allowed to ban that user.");
                                return true;
                            }

                            int BanTime = 0;
                                
                            try                                    
                            {
                                BanTime = int.Parse(Params[2]);
                            }
                            catch (FormatException) { }

                            if (BanTime <= 600)
                            {
                                Session.SendNotif("Ban time is in seconds and must be at least than 600 seconds (ten minutes). For more specific preset ban times, use the mod tool.");
                            }

                            UberEnvironment.GetGame().GetBanManager().BanUser(TargetClient, Session.GetHabbo().Username, BanTime, MergeParams(Params, 3), false);
                            return true;
                        }

                        return false;

                    case "superban":

                        if (Session.GetHabbo().HasFuse("fuse_superban"))
                        {
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(Params[1]);

                            if (TargetClient == null)
                            {
                                Session.SendNotif("User not found.");
                                return true;
                            }

                            if (TargetClient.GetHabbo().Rank >= Session.GetHabbo().Rank)
                            {
                                Session.SendNotif("You are not allowed to ban that user.");
                                return true;
                            }

                            UberEnvironment.GetGame().GetBanManager().BanUser(TargetClient, Session.GetHabbo().Username, 360000000, MergeParams(Params, 2), false);
                            return true;
                        }

                        return false;

                    case "roomkick":

                        if (Session.GetHabbo().HasFuse("fuse_roomkick"))
                        {
                            TargetRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

                            if (TargetRoom == null)
                            {
                                return false;
                            }

                            bool GenericMsg = true;
                            string ModMsg = MergeParams(Params, 1);

                            if (ModMsg.Length > 0)
                            {
                                GenericMsg = false;
                            }

                            foreach (RoomUser RoomUser in TargetRoom.UserList)
                            {
                                if (RoomUser.GetClient().GetHabbo().Rank >= Session.GetHabbo().Rank)
                                {
                                    continue;
                                }

                                if (!GenericMsg)
                                {
                                    RoomUser.GetClient().SendNotif("You have been kicked by an moderator: " + ModMsg);
                                }

                                TargetRoom.RemoveUserFromRoom(RoomUser.GetClient(), true, GenericMsg);
                            }

                            return true;
                        }

                        return false;

                    case "roomalert":

                        if (Session.GetHabbo().HasFuse("fuse_roomalert"))
                        {
                            TargetRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

                            if (TargetRoom == null)
                            {
                                return false;
                            }

                            string Msg = MergeParams(Params, 1);

                            foreach (RoomUser RoomUser in TargetRoom.UserList)
                            {
                                if (RoomUser.IsBot) { continue; }
                                RoomUser.GetClient().SendNotif(Msg);
                            }

                            return true;
                        }

                        return false;

                    case "mute":

                        if (Session.GetHabbo().HasFuse("fuse_mute"))
                        {
                            TargetUser = Params[1];
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(TargetUser);

                            if (TargetClient == null || TargetClient.GetHabbo() == null)
                            {
                                Session.SendNotif("Could not find user: " + TargetUser);
                                return true;
                            }

                            if (TargetClient.GetHabbo().Rank >= Session.GetHabbo().Rank)
                            {
                                Session.SendNotif("You are not allowed to (un)mute that user.");
                                return true;
                            }

                            TargetClient.GetHabbo().Mute();
                            return true;
                        }

                        return false;

                    case "unmute":

                        if (Session.GetHabbo().HasFuse("fuse_mute"))
                        {
                            TargetUser = Params[1];
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(TargetUser);

                            if (TargetClient == null || TargetClient.GetHabbo() == null)
                            {
                                Session.SendNotif("Could not find user: " + TargetUser);
                                return true;
                            }

                            if (TargetClient.GetHabbo().Rank >= Session.GetHabbo().Rank)
                            {
                                Session.SendNotif("You are not allowed to (un)mute that user.");
                                return true;
                            }

                            TargetClient.GetHabbo().Unmute();
                            return true;
                        }

                        return false;

                    case "alert":

                        if (Session.GetHabbo().HasFuse("fuse_alert"))
                        {
                            TargetUser = Params[1];
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(TargetUser);

                            if (TargetClient == null)
                            {
                                Session.SendNotif("Could not find user: " + TargetUser);
                                return true;
                            }

                            TargetClient.SendNotif(MergeParams(Params, 2), Session.GetHabbo().HasFuse("fuse_admin"));
                            return true;
                        }

                        return false;

                    case "softkick":
                    case "kick":

                        if (Session.GetHabbo().HasFuse("fuse_kick"))
                        {
                            TargetUser = Params[1];
                            TargetClient = UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(TargetUser);

                            if (TargetClient == null)
                            {
                                Session.SendNotif("Could not find user: " + TargetUser);
                                return true;
                            }

                            if (Session.GetHabbo().Rank <= TargetClient.GetHabbo().Rank)
                            {
                                Session.SendNotif("You are not allowed to kick that user.");
                                return true;
                            }

                            if (TargetClient.GetHabbo().CurrentRoomId < 1)
                            {
                                Session.SendNotif("That user is not in a room and can not be kicked.");
                                return true;
                            }

                            TargetRoom = UberEnvironment.GetGame().GetRoomManager().GetRoom(TargetClient.GetHabbo().CurrentRoomId);

                            if (TargetRoom == null)
                            {
                                return true;
                            }

                            TargetRoom.RemoveUserFromRoom(TargetClient, true, false);

                            if (Params.Length > 2)
                            {
                                TargetClient.SendNotif("A moderator has kicked you from the room for the following reason: " + MergeParams(Params, 2));
                            }
                            else
                            {
                                TargetClient.SendNotif("A moderator has kicked you from the room.");
                            }

                            return true;
                        }

                        return false;
                    #endregion
                }
            }
            catch { }

            return false;
        }

        public static string MergeParams(string[] Params, int Start)
        {
            StringBuilder MergedParams = new StringBuilder();

            for (int i = 0; i < Params.Length; i++)
            {
                if (i < Start)
                {
                    continue;
                }

                if (i > Start)
                {
                    MergedParams.Append(" ");
                }

                MergedParams.Append(Params[i]);
            }

            return MergedParams.ToString();
        }
    }
}
