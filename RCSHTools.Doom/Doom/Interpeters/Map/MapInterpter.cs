using System;
using System.Collections.Generic;
using System.Text;
using RCSHTools.Doom.Maps;

namespace RCSHTools.Doom.Interpeters
{
    /// <summary>
    /// A one directional 
    /// </summary>
    public class MapInterpter : LumpReader
    {
        private const int READ_NEXT = 11;

        private const int HEADER = 0;
        private const int LINEDEF = 2;
        private const int SIDEDEF = 3;
        private const int VERTEXES = 4;
        private const int SECTORS = 8;

        private Lump[] neighboors;

        private MapVerteciesInterpeter verticies;
        private MapLineInterpeter lines;
        private MapSectorInterpeter sectors;

        /// <summary>
        /// The name of the map
        /// </summary>
        public string Name => neighboors[HEADER].Name;
        public List<LineDefenition> Lines => lines.Lines;
        public List<Vertex> Vertices => verticies.Verticies;
        public List<SideDefenition> Sides => lines.Sides;
        public List<Sector> Sectors => sectors.Sectors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lump"></param>
        /// <param name="spec"></param>
        public MapInterpter(Lump lump, SpecificationMode spec) : base(lump)
        {
            neighboors = new Lump[READ_NEXT];
            
            int index = lump.File.IndexOf(lump.Name);
            for (int i = 0; i < neighboors.Length; i++)
            {
                neighboors[i] = lump.File[index + i];
            }

            verticies = new MapVerteciesInterpeter(neighboors[VERTEXES], spec);
            lines = new MapLineInterpeter(neighboors[LINEDEF], neighboors[SIDEDEF], spec);
            sectors = new MapSectorInterpeter(neighboors[SECTORS], spec);
        }
    }
}
