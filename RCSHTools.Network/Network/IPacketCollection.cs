using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Network
{
    /// <summary>
    /// Represents a collection of packets
    /// </summary>
    public interface IPacketCollection : IEnumerable<Packet>
    {
        /// <summary>
        /// The amount of packets in the collection
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets a packet from the collection by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Packet this[int index] { get; }
        /// <summary>
        /// Adds a packet for the collection
        /// </summary>
        /// <param name="packet"></param>
        void Add(Packet packet);
        /// <summary>
        /// Finds a packet in the collection
        /// </summary>
        /// <param name="predicate"></param>
        Packet Find(Predicate<Packet> predicate);
    }
}
