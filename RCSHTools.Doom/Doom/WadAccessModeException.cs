using System;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Thrown when an additive opertion is attempted on a non modifiable wad file
    /// </summary>
    [Serializable]
    public class WadAccessModeException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public WadAccessModeException(string message) : base(message) { }
    }
    /// <summary>
    /// Thrown when there is no texture pool in the 
    /// </summary>
    [Serializable]
    public class NoTexturePoolInWadException : Exception
    {   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NoTexturePoolInWadException(string message) : base(message) { }
    }
}
