using System.Windows.Forms;
using System.Net;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace LanUdpSyncServer
{
    public partial class Form1 : Form
    {
        LanTimeSyncServer t = new LanTimeSyncServer();
        IPEndPoint lep;

        public Form1()
        {
            InitializeComponent();
            richTextBox1.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1040
{\colortbl ;\red0\green0\blue0;\red0\green50\blue150;\red200\green75\blue0;\red0\green150\blue0;}
{\fonttbl{\f0\fnil\fcharset0 Lucida Console;}}
\f0\fs1\cf1 .\cf2 .\cf3 .\cf4 .\par\fs24\b Udp Sync Server\par\fs20\b0 v1.1\par\par}";
            AppendRtf(@"\cf1\fs20\b0 Listening...\par");
            t.EventHappened += t_EventHappened;
            lep = t.BeginListening("For PhisioREC on " + t.ServerName);
            if (lep != null)
            {
                string ip = lep.Address.ToString();
                AppendRtf(@"\cf2\fs20 Started\par\cf1 The interface is the " + ip + @" one.\par");
                textBox1.Text = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            }
            else
            {
                AppendRtf(@"\cf3\fs20\b failed\par");
                AppendRtf(@"\cf1\fs20\b0 Try restarting the application and ensure that the network rights are correctly set (or disable the firewall).\par");
            }
        }

        string _lastm = "";
        int _lastn = 0;

        void AppendRtf(string rtf)
        {
            Point p = richTextBox1.AutoScrollOffset;
            String t = richTextBox1.Rtf;
            t = t.Remove(t.LastIndexOf(@"\par"));
            if (rtf == _lastm)
            {
                if (_lastn == 2)
                    t += @" (2) times\par}";
                else if (_lastn > 2)
                {
                    t = t.Remove(t.LastIndexOf(" (")) + @" (" + _lastn + @") times\par\par}";
                }
                _lastn++;
            }
            else
            {
                t += String.Format(@"\cf1\fs16\b0 {0} {1}\par}}", DateTime.Now.ToLongTimeString(), rtf);

                _lastn = 1;
                _lastm = rtf;
            }
            richTextBox1.Rtf = t;
            richTextBox1.AutoScrollOffset = new Point(p.X, int.MaxValue);
        }

        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        void t_EventHappened(int arg1, string arg2)
        {
            Invoke(new Action<int, string>(t_EventHappened_h), arg1, arg2);
        }

        const byte _reqName = 36;
        const byte _reqTime = 38;

        void t_EventHappened_h(int arg1, string arg2)
        {
            switch (arg1)
            {
                case 4:
                    AppendRtf(String.Format(@"\cf3\fs20\b {0}\par", arg2));
                    break;
                case _reqName: // IP
                    AppendRtf(String.Format(@"\cf4\fs20\b0 {0} requested the name\par", arg2));
                    break;
                case _reqTime: // IP
                    AppendRtf(String.Format(@"\cf4\fs20\b0 {0} requested the time\par", arg2));
                    break;
                default:
                    AppendRtf(String.Format(@"\cf2\fs20\b0 {0}\par", arg2));
                    break;
            }
        }
    }
}
