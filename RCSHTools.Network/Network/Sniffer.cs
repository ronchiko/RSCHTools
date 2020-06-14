using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace RCSHTools.Network
{
    /// <summary>
    /// A base class for all types of sniffers
    /// </summary>
    public class Sniffer
    {
        private Socket socket;
        private byte[] buffer;
        private EndPoint endpoint;

        /// <summary>
        /// Creates a new sniffer
        /// </summary>
        public Sniffer()
        {
            endpoint = new IPEndPoint(IPAddress.Any, 0);
            buffer = new byte[1024];
            socket = new Socket(AddressFamily.InterNetwork ,SocketType.Dgram, ProtocolType.IP);
            socket.Bind(endpoint);
            socket.BeginReceive(buffer, 0, 1024, SocketFlags.None, new AsyncCallback(PacketSniffed), null);
        }

        private void PacketSniffed(IAsyncResult result)
        {
            Console.Write("Sniffed: ");
            for (int i = 0; i < 1024; i++)
            {
                Console.Write((char)buffer[i]);
            }
            Console.WriteLine();
        }
    }
}
