using System.Net.Sockets;

namespace RCSHTools.Network
{
    /// <summary>
    /// Has common network utilities
    /// </summary>
    public static class Net
    {
        public static bool Ping(string ip)
        {
            Socket socket = new Socket(SocketType.Raw, ProtocolType.Icmp);

            

            socket.Close();

            return true;
        }
    }
}
