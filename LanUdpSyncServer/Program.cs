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

    }
}
