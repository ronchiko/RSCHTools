using System;
using System.Collections.Generic;
using System.IO;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Represnets a pointer to a lump
    /// </summary>
    public struct LumpPtr
    {
        private WadFile file;

        /// <summary>
        /// The name of the lump that is pointed to
        /// </summary>
        public string Name => Lump.Name;
        /// <summary>
        /// The lump that is pointed to
        /// </summary>
        public Lump Lump { get; }

        public LumpPtr(Lump lump)
        {
            this.Lump = lump;
            file = lump.File;
        }
        public LumpPtr(uint start, uint size, byte[] nameBuffer, WadFile file)
        {
            this.file = file;
            string name = DoomUtils.ToName(nameBuffer);

            Lump = file.AllocateLump(name, start, size);
        }
    }
}
