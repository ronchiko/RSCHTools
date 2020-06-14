using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Doom
{
    /// <summary>
    /// The <see cref="WadFile"/> type
    /// </summary>
    public enum WadType
    {
        /// <summary>
        /// Represnets an IWAD wad file
        /// </summary>
        IWad,
        /// <summary>
        /// Represnets a PWAD wad file
        /// </summary>
        PWad
    }
    /// <summary>
    /// How should the wad file be accessed
    /// </summary>
    public enum WadReadMode
    {
        /// <summary>
        /// The wad can only be read from
        /// </summary>
        Readonly,
        /// <summary>
        /// The wad can be overriden with PWad files, but cant be saved
        /// </summary>
        Extendable,
        /// <summary>
        /// The wad can modified, but not overriden by other Pwad files
        /// </summary>
        Writable,
    }
}
