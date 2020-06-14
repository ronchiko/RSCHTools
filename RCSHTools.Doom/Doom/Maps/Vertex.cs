using System;
using System.Collections.Generic;
using ByteStream = System.IO.MemoryStream;

namespace RCSHTools.Doom.Maps
{
    /// <summary>
    /// Represents a coordinate in a map
    /// https://zdoom.org/wiki/Vertex
    /// </summary>
    public struct Vertex : IMapItem
    {
        /// <summary>
        /// The regex used to parse UDMF map vertecies
        /// </summary>
        public const string VERTEX_REGEX = @"vertex \/\/(.*)\n{\nx = ([0-9\.-]*);\ny = ([0-9\.-]*);\n}";

        public short X { get; set; }
        public short Y { get; set; }

        public int UDMF_INDEX { get; set; }

        public Vertex(short x, short y)
        {
            X = x;
            Y = y;
            UDMF_INDEX = 0;
        }
        public Vertex(ByteStream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            X = BitConverter.ToInt16(buffer, 0);
            Y = BitConverter.ToInt16(buffer, 2);
            UDMF_INDEX = 0;
        }

        void IMapItem.Write(ByteStream stream, SpecificationMode spec)
        {
            switch (spec)
            {
                case SpecificationMode.Doom:
                case SpecificationMode.Hexen:
                    stream.Write(BitConverter.GetBytes(X), 0, 2);
                    stream.Write(BitConverter.GetBytes(Y), 0, 2);
                    break;
                case SpecificationMode.UDMF:
                    byte[] buffer = System.Text.Encoding.ASCII.GetBytes("vertex \\\\ " + UDMF_INDEX + "\n{\nx = " + X + ";\ny = " + Y + ";\n}");
                    stream.Write(buffer, 0, buffer.Length);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
