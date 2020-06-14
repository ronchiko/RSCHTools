using System;
using System.Drawing;
using System.IO;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Used to handle all types of graphics
    /// </summary>
    public class Graphic
    {
        private static readonly byte[] PNG_ID = { 0x50, 0x4E, 0x47 }; 
        private static readonly byte[] JFIF_ID = { 0x4A, 0x46, 0x49, 0x46, 0 };     // JFIF null terminted (In ascii)

        /// <summary>
        /// What kind of image it is
        /// </summary>
        public GraphicType Type { get; }

        private readonly DoomGraphic doomImage;
        private Lump lump;

        /// <summary>
        /// Loads a new graphics from a wad
        /// </summary>
        /// <param name="lumpName"></param>
        /// <param name="file"></param>
        public Graphic(WadFile file, string lumpName)
        {
            lump = file[lumpName];

            using (MemoryStream stream = lump.RawStream())
            {
                // Move to png marker
                stream.ReadByte();

                bool png = IsPng(stream);
                bool jfif = false;

                if (!png)
                {
                    stream.Position = 4;
                    for (int i = 0; i < 2; i++) stream.ReadByte();
                    jfif = IsJfif(stream);
                }

                if (png) Type = GraphicType.Png;
                else if (jfif) Type = GraphicType.Jpeg;
                else
                {
                    Type = GraphicType.DoomPatch;
                    doomImage = new DoomGraphic(lump, file.Pallete(0));
                }
            }
        }

        private bool IsPng(MemoryStream stream)
        {
            for (int i = 0; i < PNG_ID.Length; i++)
            {
                if (stream.ReadByte() != PNG_ID[i]) return false;
            }
            return true;
        }

        private bool IsJfif(MemoryStream stream)
        {
            for (int i = 0; i < JFIF_ID.Length; i++)
            {
                int b = stream.ReadByte();
                if (b != JFIF_ID[i]) return false;
            }
            return true;
        }
    
        /// <summary>
        /// Loads the graphic as a bitmap. <b>Requires System.Drawing.Common</b>
        /// </summary>
        /// <returns></returns>
        public Bitmap LoadImage()
        {
            switch (Type)
            {
                case GraphicType.Png:
                case GraphicType.Jpeg:
                    return new Bitmap(lump.RawStream());
                case GraphicType.DoomPatch:
                    return doomImage.ToImage();
                default:
                    throw new ArgumentException("Unsupported image type");
            }
        }
    
        /// <summary>
        /// The lumps raw data
        /// </summary>
        /// <returns></returns>
        public byte[] Raw()
        {
            return lump.CopyRaw();
        }

        /// <summary>
        /// The prepared doom graphic, returns null if the graphic type is not <see cref="GraphicType.DoomPatch"/>
        /// </summary>
        /// <returns></returns>
        public DoomGraphic DoomGraphic() => doomImage;
    }

    /// <summary>
    /// What kind of graphic is it
    /// </summary>
    public enum GraphicType
    {
        /// <summary>
        /// The file is compressed in PNG format
        /// </summary>
        Png,
        /// <summary>
        /// The file is compressed in JPEG format
        /// </summary>
        Jpeg,
        /// <summary>
        /// The file is a doom compressed file
        /// </summary>
        DoomPatch
    }
}
