using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Network
{
    /// <summary>
    /// Represents a packet layer
    /// </summary>
    public interface IPacketLayer
    {
        /// <summary>
        /// Are the properties of this layer are accessible for the user. 
        /// Some lower level packets cannot be accesed without a Raw Socket support in machines
        /// </summary>
        bool IsVirtual { get; set; }
        
    }
}
