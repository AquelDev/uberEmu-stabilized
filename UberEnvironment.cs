using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uber.Core;
using Uber.Storage;
using Uber.Net;
using Uber.HabboHotel;
using MySql.Data.MySqlClient;
using Uber.Communication;

namespace Uber
{
    class UberEnvironment
    {
        private static Logging Logging;
        private static ConfigurationData Configuration;
        private static DatabaseManager DatabaseManager;
        private static Encoding DefaultEncoding;
        private static PacketManager _packetManager = new PacketManager();
        private static TcpConnectionManager ConnectionManager;
        private static MusSocket MusSocket;
        private static Game Game;

        public static string PrettyVersion
        {
            get
            {
                return "uberEmulator Build 1.0.0 - Revised";
            }
        }

        public static void Initialize()
        {
            

            Console.Title = PrettyVersion;
            Console.Clear();
            Console.WriteLine("                                                                      ");
            Console.WriteLine("               |              ,---.          |         |              ");
            Console.WriteLine("          .   .|---.,---.,---.|--- ,-.-..   .|    ,---.|--- ,---.,---.");
            Console.WriteLine("          |   ||   ||---'|    |    | | ||   ||    ,---||    |   ||    ");
            Console.WriteLine("          `---'`---'`---'`    `---'` ' '`---'`---'`---^`---'`---'`    ");
            Console.WriteLine("                                                                      ");
            Console.WriteLine("                                   " + PrettyVersion);
            Console.WriteLine("                                                                      ");
            Console.WriteLine("       ------------------------------------------------------------------");
            Console.WriteLine("                                                                      ");

            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WindowWidth = 100;
            Console.WindowHeight = 30;
            

            DefaultEncoding = Encoding.Default;

            Logging = new Logging();
            Logging.MinimumLogLevel = LogLevel.Debug;

            try
            {
                Configuration = new ConfigurationData("config.conf");

                if (UberEnvironment.GetConfig().data["db.password"].Length == 0)
                {
                    throw new Exception("For security reasons, your MySQL password cannot be left blank. Please change your password to start the server.");
                }

                DatabaseServer dbServer = new DatabaseServer(
                    UberEnvironment.GetConfig().data["db.hostname"],
                    uint.Parse(UberEnvironment.GetConfig().data["db.port"]),
                    UberEnvironment.GetConfig().data["db.username"],
                    UberEnvironment.GetConfig().data["db.password"]);

                Database db = new Database(
                    UberEnvironment.GetConfig().data["db.name"],
                    uint.Parse(UberEnvironment.GetConfig().data["db.pool.minsize"]),
                    uint.Parse(UberEnvironment.GetConfig().data["db.pool.maxsize"]));

                DatabaseManager = new DatabaseManager(dbServer, db);

                


                MusSocket = new MusSocket(UberEnvironment.GetConfig().data["mus.tcp.bindip"],
                    int.Parse(UberEnvironment.GetConfig().data["mus.tcp.port"]),
                    UberEnvironment.GetConfig().data["mus.tcp.allowedaddr"].Split(';'), 20);

                Game = new Game();
                

                ConnectionManager = new TcpConnectionManager(
                    UberEnvironment.GetConfig().data["game.tcp.bindip"],
                    int.Parse(UberEnvironment.GetConfig().data["game.tcp.port"]),
                    int.Parse(UberEnvironment.GetConfig().data["game.tcp.conlimit"]));
                ConnectionManager.GetListener().Start();

                UberEnvironment.GetLogging().WriteLine("The environment has initialized successfully. Ready for connections.");
            }

            catch (KeyNotFoundException)
            {
                Logging.WriteLine("Please check your configuration file - some values appear to be missing.", LogLevel.Error);
                Logging.WriteLine("Press any key to shut down ...", LogLevel.Error);

                Console.ReadKey(true);
                UberEnvironment.Destroy();

                return;
            }

            catch (InvalidOperationException e)
            {
                Logging.WriteLine("Failed to initialize: " + e.Message, LogLevel.Error);
                Logging.WriteLine("Press any key to shut down ...", LogLevel.Error);

                Console.ReadKey(true);
                UberEnvironment.Destroy();

                return;
            }
        }

        public static bool EnumToBool(string Enum)
        {
            if (Enum == "1")
            {
                return true;
            }

            return false;
        }

        public static string BoolToEnum(bool Bool)
        {
            if (Bool)
            {
                return "1";
            }

            return "0";
        }

        public static int GetRandomNumber(int Min, int Max)
        {
            RandomBase Quick = new Quick();
            return Quick.Next(Min, Max);
        }

        public static Double GetUnixTimestamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }

        public static string FilterInjectionChars(string Input)
        {
            return FilterInjectionChars(Input, false);
        }

        public static string FilterInjectionChars(string Input, bool AllowLinebreaks)
        {
            Input = Input.Replace(Convert.ToChar(1), ' ');
            Input = Input.Replace(Convert.ToChar(2), ' ');
            Input = Input.Replace(Convert.ToChar(3), ' '); // Was commented
            Input = Input.Replace(Convert.ToChar(9), ' ');

            if (!AllowLinebreaks)
            {
                Input = Input.Replace(Convert.ToChar(13), ' ');
            }

            return Input;
        }

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            if (string.IsNullOrEmpty(inputStr))
            {
                return false;
            }

            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!(char.IsLetter(inputStr[i])) && (!(char.IsNumber(inputStr[i]))))
                {
                    return false;
                }
            }

            return true;
        }

        public static ConfigurationData GetConfig()
        {
            return Configuration;
        }

        public static Logging GetLogging()
        {
            return Logging;
        }

        public static DatabaseManager GetDatabase()
        {
            return DatabaseManager;
        }

        public static Encoding GetDefaultEncoding()
        {
            return DefaultEncoding;
        }

        public static PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public static TcpConnectionManager GetConnectionManager()
        {
            return ConnectionManager;
        }

        public static Game GetGame()
        {
            return Game;
        }

        public static void Destroy()
        {
            UberEnvironment.GetLogging().WriteLine("Destroying environment...");

            if (GetGame() != null)
            {
                GetGame().Destroy();
                Game = null;
            }

            if (GetConnectionManager() != null)
            {
                UberEnvironment.GetLogging().WriteLine("Destroying connection manager.");
                GetConnectionManager().GetListener().Stop();
                GetConnectionManager().GetListener().Destroy();
                GetConnectionManager().DestroyManager();
                ConnectionManager = null;
            }

            if (GetDatabase() != null)
            {
                UberEnvironment.GetLogging().WriteLine("Destroying database manager.");
                MySqlConnection.ClearAllPools();
                DatabaseManager = null;
            }

            Logging.WriteLine("Uninitialized successfully. Closing.");

            Environment.Exit(0);
        }
    }

   
}
