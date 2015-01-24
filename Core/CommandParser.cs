using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uber.Messages;

namespace Uber.Core
{
    class CommandParser
    {
        public static void Parse(string Input)
        {
            string[] Params = Input.Split(' ');

            switch (Params[0])
            {
                case "reload_models":

                    UberEnvironment.GetGame().GetRoomManager().LoadModels();
                    break;

                case "reload_bans":

                    UberEnvironment.GetGame().GetBanManager().LoadBans();
                    break;

                case "reload_navigator":

                    UberEnvironment.GetGame().GetNavigator().Initialize();
                    UberEnvironment.GetLogging().WriteLine("Re-initialized navigator successfully.");

                    break;

                case "reload_items":

                    UberEnvironment.GetGame().GetItemManager().LoadItems();
                    UberEnvironment.GetLogging().WriteLine("Please note that changes may not be reflected immediatly in currently loaded rooms.");

                    break;

                case "reload_help":

                    UberEnvironment.GetGame().GetHelpTool().LoadCategories();
                    UberEnvironment.GetGame().GetHelpTool().LoadTopics();
                    UberEnvironment.GetLogging().WriteLine("Reloaded help categories and topics successfully.");

                    break;

                case "reload_catalog":

                    UberEnvironment.GetGame().GetCatalog().Initialize();
                    UberEnvironment.GetGame().GetClientManager().BroadcastMessage(new ServerPacket(441));
                    UberEnvironment.GetLogging().WriteLine("Published catalog successfully.");

                    break;

                case "reload_roles":

                    UberEnvironment.GetGame().GetRoleManager().LoadRoles();
                    UberEnvironment.GetGame().GetRoleManager().LoadRights();
                    UberEnvironment.GetLogging().WriteLine("Reloaded ranks and rights successfully.");

                    break;

                case "cls":

                    UberEnvironment.GetLogging().Clear();
                    break;
                case "about":

                    UberEnvironment.GetLogging().WriteLine("This emulator was created by Method and Nillus, and carried on by PowahAlert");

                    break;

                case "help":

                    UberEnvironment.GetLogging().WriteLine("Available commands are: about, cls, close, help, reload_catalog, reload_navigator, reload_roles, reload_help, reload_items");

                    break;

                case "close":

                    UberEnvironment.Destroy();

                    break;

                default:

                    UberEnvironment.GetLogging().WriteLine("Unrecognized command or operation: " + Input + ". Use 'help' for a list of available commands.", LogLevel.Warning);

                    break;
            }
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
