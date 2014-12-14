using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

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
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            if (Process.GetCurrentProcess().PriorityClass != ProcessPriorityClass.RealTime
                && Process.GetCurrentProcess().PriorityClass != ProcessPriorityClass.High)
            {
                MessageBox.Show("The process priority can not be set.\nThis means that the actual sync accuracy stability is not granted as before.\nDouble compute and double check the offset.", "Warning");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
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
