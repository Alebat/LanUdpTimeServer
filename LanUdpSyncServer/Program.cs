using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LanUdpSyncServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new Form1());

            LanTimeSyncServer s = new LanTimeSyncServer();
            s.BeginListening("ServierOne");
            Console.ReadLine();
        }

        public static double ToUnixTimestamp(this DateTime value)
        {
            return (double)(value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
        }

        public static byte[] GetBytes(this long argument)
        {
            byte[] byteArray = BitConverter.GetBytes(argument);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);
            return byteArray;
        }
    }
}
