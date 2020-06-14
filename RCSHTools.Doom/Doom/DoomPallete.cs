using System;
using System.Collections.Generic;
using System.IO;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Represnts a single doom pallete
    /// </summary>
    public class DoomPallete
    {
        private RGB[] colors;

        /// <summary>
        /// Returns the colour with thee correct index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RGB this[int index] => colors[index];

        /// <summary>
        /// Createss a new doom color pallete
        /// </summary>
        /// <param name="stream"></param>
        public DoomPallete(MemoryStream stream)
        {
            colors = new RGB[256];

            for (int i = 0; i < 256 && stream.Position < stream.Length; i++)
            {
                int r = stream.ReadByte();
                int g = stream.ReadByte();
                int b = stream.ReadByte();

                colors[i] = new RGB((byte)r, (byte)g, (byte)b);
            }
        }
    }

    /// <summary>
    /// A collection of Palletes (PLAYPAL)
    /// </summary>
    public class DoomPalleteCollection
    {
        private List<DoomPallete> palletes;

        /// <summary>
        /// Returns a pallete by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DoomPallete this[int index] => palletes[index];

        /// <summary>
        /// Creates a new pallete collection
        /// </summary>
        /// <param name="stream"></param>
        public DoomPalleteCollection(MemoryStream stream)
        {
            palletes = new List<DoomPallete>();
            while(stream.Length > stream.Position)
            {
                palletes.Add(new DoomPallete(stream));
            }
        }


    }
}
