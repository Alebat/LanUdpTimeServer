using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace LanUdpSyncServer
{
    public partial class Form1 : Form
    {
        const int listenPort = 12865;
        const byte _reqName = 36;
        const byte _reqTime = 38;
        const byte _resTime = 39;

        int pings = 0;

        public Form1()
        {
            listener.BeginReceive(new AsyncCallback(received), listener);
            InitializeComponent();
        }

        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any,listenPort);
        string serverName = "time-server-from-" + DateTime.Now.ToShortTimeString().Replace(":", "-");

        // 0 1 2 3 4  5 6 7 8
        // R t1t1t1t1 t2t2t2t2

        void received(IAsyncResult r)
        {
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            byte[] b = ((UdpClient)r.AsyncState).EndReceive(r, ref e);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            long t2 = (long)(DateTime.Now.ToUnixTimestamp() * 1000);
            if (b[0] == _reqTime)
            {
                long t1 = BitConverter.ToInt64(b, 1);

                b = new byte[1];
                b[0] = _resTime;
                b = b.Concat((t2 - t1).GetBytes()).ToArray();
                Console.WriteLine("t1:{0}", t1);
                Console.WriteLine("t2:{0}", t2);
                Console.WriteLine("df:{0}", t2-t1);

                long t3 = (long)(DateTime.Now.ToUnixTimestamp() * 1000);
                Console.WriteLine("t3:{0}", t3);
                b = b.Concat(t3.GetBytes()).ToArray();
                e.Port = listenPort;
                listener.Send(b, b.Length, e);
                listener.BeginReceive(new AsyncCallback(received), listener);
                pings++;
                Console.WriteLine("pings:{0}\n", pings);
            }
            else if (b[0] == _reqName)
            {
                listener.BeginReceive(new AsyncCallback(received), listener);
                b = Encoding.ASCII.GetBytes(serverName);
                listener.Send(b, b.Length, e);
            }
            
        }

        private void buttonSetName_Click(object sender, EventArgs e)
        {
            serverName = ((Button)sender).Text;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            label1.Text = pings.ToString();
        }
    }
}
