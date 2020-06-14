using System.Collections;
using System.Collections.Generic;
using System.IO;
using RCSHTools.Doom.Maps;

namespace RCSHTools.Doom.Interpeters
{
    public class MapLineInterpeter : LumpReader
    {
        /// <summary>
        /// The line defenitions
        /// </summary>
        public List<LineDefenition> Lines { get; }
        /// <summary>
        /// The side defenitions
        /// </summary>
        public List<SideDefenition> Sides { get; }
        /// <summary>
        /// Creates a new map line interpeter
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="sides"></param>
        /// <param name="mode"></param>
        public MapLineInterpeter(Lump lines, Lump sides, SpecificationMode mode) : base(lines)
        {
            // The size of a single side is 30 bytes
            int[] arr_sideLinks = new int[sides.Size / 30]; 
            using(MemoryStream stream = new MemoryStream(Raw))
            {
                Lines = new List<LineDefenition>();

                int lineIndex = 0;
                while(stream.Position < stream.Length)
                {
                    LineDefenition line = new LineDefenition(stream, mode);
                    Lines.Add(line);

                    if(line.HasLeft())
                        arr_sideLinks[line.LeftSideDef] = lineIndex;
                    if (line.HasRight())
                        arr_sideLinks[line.RightSideDef] = lineIndex;

                    lineIndex++;
                }
            }

            using(MemoryStream stream = new MemoryStream(new LumpReader(sides).Raw))
            {
                Sides = new List<SideDefenition>();
                int sideIndex = 0;

                while (stream.Position < stream.Length)
                {
                    Sides.Add(new SideDefenition(stream, mode, Lines[arr_sideLinks[sideIndex]]));
                    sideIndex++;
                }
            }
        }
    }
}
