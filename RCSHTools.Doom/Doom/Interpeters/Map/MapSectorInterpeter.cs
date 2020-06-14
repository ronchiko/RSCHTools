using System;
using System.Collections.Generic;
using System.IO;
using RCSHTools.Doom.Maps;

namespace RCSHTools.Doom.Interpeters
{
    public class MapSectorInterpeter : LumpReader
    {
        public List<Sector> Sectors { get; }

        public MapSectorInterpeter(Lump lump, SpecificationMode mode) : base(lump)
        {
            using (MemoryStream stream = new MemoryStream(Raw))
            {
                Sectors = new List<Sector>();
                while(stream.Position < stream.Length){
                    Sectors.Add(new Sector(stream, mode));
                }
            }
        }
    }
}
