using System;
using System.IO;
using System.Text;

namespace RCSHTools.Doom.Maps
{
    /// <summary>
    /// Represents a map sector.
    /// See: https://zdoom.org/wiki/Sector
    /// </summary>
    public struct Sector
    {
        /// <summary>
        /// The floor height
        /// </summary>
        public short FloorHeight { get; set; }
        /// <summary>
        /// The ceiling height
        /// </summary>
        public short CeilingHeight { get; set; }
        /// <summary>
        /// The texture of the floor
        /// </summary>
        public string FloorTexture { get; set; }
        /// <summary>
        /// The texture of ceiling
        /// </summary>
        public string CeilingTexture { get; set; }
        /// <summary>
        /// The Light level of the sector
        /// </summary>
        public short LightLevel { get; set; }
        
        public ushort SectorSpecial { get; set; }
        public ushort SectorTag { get; set; }

        public Sector(MemoryStream stream, SpecificationMode spec)
        {
            switch (spec)
            {
                case SpecificationMode.Doom:
                case SpecificationMode.Hexen:
                    byte[] buffer = new byte[26];
                    stream.Read(buffer, 0, 26);
                    FloorHeight = BitConverter.ToInt16(buffer, 0);
                    CeilingHeight = BitConverter.ToInt16(buffer, 2);
                    FloorTexture = DoomUtils.ToName(buffer, 4, 8);
                    CeilingTexture = DoomUtils.ToName(buffer, 12, 8);
                    LightLevel = BitConverter.ToInt16(buffer, 20);
                    SectorSpecial = BitConverter.ToUInt16(buffer, 22);
                    SectorTag = BitConverter.ToUInt16(buffer, 24);
                    break;
                case SpecificationMode.UDMF:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
