using System;
using System.IO;
using System.Text;

namespace RCSHTools.Doom.Maps
{
    /// <summary>
    /// Represnts a walls Sidedef
    /// See: https://zdoom.org/wiki/Sidedef
    /// </summary>
    public struct SideDefenition : IMapItem
    {
        private LineDefenition line;

        /// <summary>
        /// The texture's x offset
        /// </summary>
        public short OffsetX { get; set; }
        /// <summary>
        /// The texture's y offset
        /// </summary>
        public short OffsetY { get; set; }
        /// <summary>
        /// The top texture of the side
        /// </summary>
        public string TopTexture { get; set; }
        /// <summary>
        /// The bottom texture of the side
        /// </summary>
        public string BottomTexture { get; set; }
        /// <summary>
        /// The middle texture of the side
        /// </summary>
        public string Texture { get; set; }
        /// <summary>
        /// The index of the sides sector
        /// </summary>
        public ushort Sector { get; set; }

        /// <summary>
        /// Index of the start vertex
        /// </summary>
        public ushort Start => line.StartIndex;
        /// <summary>
        /// Index of the end vertex
        /// </summary>
        public ushort End => line.EndIndex;

        /// <summary>
        /// Creates a side defenition from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="spec"></param>
        /// <param name="line"></param>
        public SideDefenition(MemoryStream stream, SpecificationMode spec, LineDefenition line)
        {
            this.line = line;
            switch (spec)
            {
                case SpecificationMode.Doom:
                case SpecificationMode.Hexen:
                    byte[] buffer = new byte[30];
                    stream.Read(buffer, 0, 30);
                    OffsetX = BitConverter.ToInt16(buffer, 0);
                    OffsetY = BitConverter.ToInt16(buffer, 2);
                    TopTexture = DoomUtils.ToName(buffer, 4, 8);
                    BottomTexture = DoomUtils.ToName(buffer, 12, 8);
                    Texture = DoomUtils.ToName(buffer, 20, 8);
                    Sector = BitConverter.ToUInt16(buffer, 28);
                    break;
                case SpecificationMode.UDMF:
                default:
                    throw new NotImplementedException();
            }
        }

        void IMapItem.Write(MemoryStream stream, SpecificationMode spec)
        {
            switch (spec)
            {
                case SpecificationMode.Doom:
                case SpecificationMode.Hexen:
                    stream.Write(BitConverter.GetBytes(OffsetX), 0, 2);
                    stream.Write(BitConverter.GetBytes(OffsetY), 0, 2);
                    stream.Write(DoomUtils.GetNameAsBytes(TopTexture), 0, 8);
                    stream.Write(DoomUtils.GetNameAsBytes(BottomTexture), 0, 8);
                    stream.Write(DoomUtils.GetNameAsBytes(Texture), 0, 8);
                    stream.Write(BitConverter.GetBytes(Sector), 0, 2);
                    break;
                case SpecificationMode.UDMF:
                default:
                    throw new NotImplementedException();
            }
        }

    }

    namespace UDMF
    {
        public enum UDMFSideDefenitionProperties
        {
            /// <summary>
            /// The alpha of the wall (alpha:<see cref="float"/>
            /// </summary>
            Alpha,

        }
    }
}
