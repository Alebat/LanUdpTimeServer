using System;

namespace LanUdpSyncServer
{
    public static class Extensions
    {
        public static double ToUnixTimestamp(this DateTime value)
        {
            return (double)(value - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
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
