using System;
using System.Collections.Generic;
using System.Text;
using RCSHTools.Doom.Maps;

namespace RCSHTools.Doom.Interpeters
{
    public class MapVerteciesInterpeter : LumpReader
    {
        public List<Vertex> Verticies { get; }

        public MapVerteciesInterpeter(Lump lump, SpecificationMode mode) : base(lump)
        {
            Verticies = new List<Vertex>();
            switch (mode)
            {
                case SpecificationMode.Doom:
                case SpecificationMode.Hexen:
                    for (int i = 0; i < Raw.Length; i += 4)
                    {
                        short x = BitConverter.ToInt16(Raw, i);
                        short y = BitConverter.ToInt16(Raw, i + 2);

                        Verticies.Add(new Vertex(x, y));
                    }
                    break;
                case SpecificationMode.UDMF:
                default:
                    throw new NotImplementedException();
            }            
        }
    }
}
