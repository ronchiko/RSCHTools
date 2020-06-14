using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Network.RISP
{
    /// <summary>
    /// An enum that represents all the error that can occur on a RISP clients and servers
    /// </summary>
    public enum RISPErrors
    {
        /// <summary>
        /// No errors occured
        /// </summary>
        Successful,
        /// <summary>
        /// The image data supplied for the pairing is invalid
        /// </summary>
        InvalidImageData,
        /// <summary>
        /// When trying to pair an image that is too large
        /// </summary>
        ResolutionTooLarge,
        /// <summary>
        /// A socket error accured
        /// </summary>
        SocketError,
        /// <summary>
        /// The pairing was declined by the server
        /// </summary>
        PairDeclined,
        /// <summary>
        /// When the server recieves diffrent resultions from a client
        /// </summary>
        DimensionMismatch,
        /// <summary>
        /// An unknown error occured
        /// </summary>
        Unknown
    }
}
