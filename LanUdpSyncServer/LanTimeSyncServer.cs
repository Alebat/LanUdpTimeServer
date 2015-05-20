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

        public IPEndPoint BeginListening()
        {
            try
            {
                Recall(_listenerName);
                Recall(_listenerTime);
                ServerName = ((IPEndPoint)_listenerTime.Client.LocalEndPoint).Address.ToString();
                return (IPEndPoint)_listenerTime.Client.LocalEndPoint;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IPEndPoint BeginListening(string serverName)
        {
            IPEndPoint r = BeginListening();
            this._serverName = serverName;
            return r;
        }

        void Received(IAsyncResult r)
        {
            long t2 = (long)(DateTime.UtcNow.ToUnixTimestamp() * 1000);
            IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
            byte[] b = ((UdpClient)r.AsyncState).EndReceive(r, ref e);
            
            if (b[0] == _reqTime)
            {
                OnEventHappened(_reqTime, e.Address.ToString() + ":" + e.Port.ToString());
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

                e.Port = timePort;
                long t3 = (long)(DateTime.UtcNow.ToUnixTimestamp() * 1000);
                b = b.Concat(t3.GetBytes()).ToArray();
                ((UdpClient)r.AsyncState).Send(b, b.Length, e);
                Recall(r.AsyncState);
                _pings++;
            }
            else if (b[0] == _reqName)
            {
                OnEventHappened(_reqName, e.Address.ToString() + ":" + e.Port.ToString());
                Recall(r.AsyncState);
                b = Encoding.ASCII.GetBytes(_serverName);
                ((UdpClient)r.AsyncState).Send(b, b.Length, new IPEndPoint(e.Address, nameAnsPort));
            }
            else
            {
                OnEventHappened(4, "Problem: received a " + b[0] + " there may be an other network application that works on the same channel.");
                Recall(r.AsyncState);
            }
            
        }

        private void Recall(object r)
        {
            ((UdpClient)r).BeginReceive(new AsyncCallback(Received), ((UdpClient)r));
        }

        void OnEventHappened(int i, string m)
        {
            if (EventHappened != null)
                EventHappened.Invoke(i, m);
        }

        public event Action<int, string> EventHappened;
	}
}
