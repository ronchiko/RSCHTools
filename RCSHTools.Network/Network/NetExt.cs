using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace RCSHTools.Network
{
    /// <summary>
    /// Extension for Network tools
    /// </summary>
    public static class NetExt
    {
        /// <summary>
        /// Receives a value from a <see cref="Socket"/> into a <see cref="NetBuffer"/>
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        public static void Receive(this Socket socket, NetBuffer buffer)
        {
            buffer.Receive(socket);
        }
        /// <summary>
        /// <inheritdoc cref="Receive(Socket, NetBuffer)"/>
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        public static void ReceiveFrom(this Socket socket, NetBuffer buffer, ref EndPoint remote)
        {
            buffer.ReceiveFrom(socket, ref remote);
        }
        /// <summary>
        /// Sends data to a <see cref="Socket"/> from a <see cref="NetBuffer"/>
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        public static void Send(this Socket socket, NetBuffer buffer)
        {
            buffer.Send(socket);
        }
        /// <summary>
        /// <inheritdoc cref="Send(Socket, NetBuffer)"/>
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remote"></param>
        /// <param name="buffer"></param>
        public static void SendTo(this Socket socket, NetBuffer buffer, EndPoint remote)
        {
            buffer.SendTo(socket, remote);
        }
    }
}
