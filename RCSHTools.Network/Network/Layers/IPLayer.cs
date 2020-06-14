using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RCSHTools.Network
{
    /// <summary>
    /// Represents an IP layer
    /// </summary>
    public struct IPLayer
    {
        /// <summary>
        /// IP protocol version
        /// </summary>
        public byte Version { get; set; }
        /// <summary>
        /// Length of the header
        /// </summary>
        public byte HeaderLength { get; set; }
        /// <summary>
        /// Service type
        /// </summary>
        public short Service { get; set; }
        /// <summary>
        /// The total length of the packet
        /// </summary>
        public int Length { get; }
        /// <summary>
        /// Packet id
        /// </summary>
        public int ID { get; set; }

        // Flags and fragment goto in the next 4 bytes

        /// <summary>
        /// Time to live
        /// </summary>
        public short TTL { get; set; }
        /// <summary>
        /// The protocol that is used
        /// </summary>
        public ProtocolType Protocol { get; set; }
        /// <summary>
        /// The packets IP header checksum
        /// </summary>
        public int Checksum { get; private set; }

        private void CalculateChecksum()
        {

        }
    }
}
