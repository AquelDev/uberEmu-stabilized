using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Permissions;
using System.IO;

using Uber.Core;

namespace Uber
{
    public class Program
    {
        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlAppDomain)]
        public static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            try
            {
                UberEnvironment.Initialize();

                while (true)
                {
                    CommandParser.Parse(Console.ReadLine());
                }
            }

            catch (Exception e)
            {
                Console.Write(e.Message);
                Console.ReadKey(true);
            }
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            #region Logging
            #region Write Errors to a log file
            // Create a writer and open the file:
            StreamWriter log;

            if (!File.Exists("errorlog.txt"))
            {
                log = new StreamWriter("errorlog.txt");
            }
            else
            {
                log = File.AppendText("errorlog.txt");
            }

            // Write to the file:
            log.WriteLine(DateTime.Now);
            log.WriteLine(e.ToString());
            log.WriteLine();

            // Close the stream:
            log.Close();
            #endregion
            #endregion

            Console.WriteLine("Unhandled Exception : " + e.ToString());
            Console.ReadKey(true);
        }
    }
}
