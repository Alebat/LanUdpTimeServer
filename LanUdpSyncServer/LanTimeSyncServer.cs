using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LanUdpSyncServer
{
    class LanTimeSyncServer
    {
        const int timePort = 12865;
        const int namePort = 12866;
        const int nameAnsPort = 12867;

        const byte _reqName = 36;
        const byte _reqTime = 38;
        const byte _resTime = 39;

        UdpClient _listenerTime = new UdpClient(timePort);
        UdpClient _listenerName = new UdpClient(namePort);
        IPEndPoint _groupEP = new IPEndPoint(IPAddress.Any,timePort);
        string _serverName = "time-server-from-" + DateTime.Now.ToShortTimeString().Replace(":", "-");

        int _pings = 0;

        public int Pings
        {
            get { return _pings; }
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        // 0 1 2 3 4  5 6 7 8
        // R t1t1t1t1 t2t2t2t2

        public void BeginListening()
        {
            Console.Write("Listening..");
            Recall(_listenerName);
            Recall(_listenerTime);
            Console.WriteLine("started");
        }

        public void BeginListening(string serverName)
        {
            this._serverName = serverName;
            BeginListening();
        }

        void Received(IAsyncResult r)
        {
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            byte[] b = ((UdpClient)r.AsyncState).EndReceive(r, ref e);
            long t2 = (long)(DateTime.Now.ToUnixTimestamp() * 1000);
            Console.Write("[{0}] {1}:{2} Request ", DateTime.Now.ToUnixTimestamp(), e.Address.ToString(), e.Port.ToString());
            if (b[0] == _reqTime)
            {
                Console.WriteLine("_reqTime");
                long t1;
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(b);
                    t1 = BitConverter.ToInt64(b, b.Length - 9);
                }
                else
                    t1 = BitConverter.ToInt64(b, 1);

                b = new byte[1];
                b[0] = _resTime;
                b = b.Concat((t2 - t1).GetBytes()).ToArray();

                long t3 = (long)(DateTime.Now.ToUnixTimestamp() * 1000);
                b = b.Concat(t3.GetBytes()).ToArray();
                e.Port = timePort;
                ((UdpClient)r.AsyncState).Send(b, b.Length, e);
                Recall(r.AsyncState);
                _pings++;
                if (_pings == 100)
                    Console.WriteLine("100th ping", _pings);
            }
            else if (b[0] == _reqName)
            {
                Console.WriteLine("_reqName");
                Recall(r.AsyncState);
                b = Encoding.ASCII.GetBytes(_serverName);
                ((UdpClient)r.AsyncState).Send(b, b.Length, new IPEndPoint(e.Address, nameAnsPort));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Received {0}", b[0]);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Recall(r.AsyncState);
            }
            
        }

        private void Recall(object r)
        {
            ((UdpClient)r).BeginReceive(new AsyncCallback(Received), ((UdpClient)r));
        }

	}
}
