using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Represents a doom format graphic
    /// </summary>
    public class DoomGraphic
    {

        /// <summary>
        /// Widths of the image
        /// </summary>
        public ushort Width { get; }
        /// <summary>
        /// Height of the image
        /// </summary>
        public ushort Height { get; }
        /// <summary>
        /// The x offset of the graphic
        /// </summary>
        public short OffsetX { get; set; }
        /// <summary>
        /// The Y offset of the graphic
        /// </summary>
        public short OffsetY { get; set; }

        private int[] columns;
        private DoomPallete pallete;
        private byte[] lump;

        /// <summary>
        /// Creates a new doom graphic
        /// </summary>
        /// <param name="lump"></param>
        /// <param name="pallete"></param>
        public DoomGraphic(Lump lump, DoomPallete pallete)
        {
            Width = (ushort)lump.ReadInt16(0);
            Height = (ushort)lump.ReadInt16(2);
            OffsetX = lump.ReadInt16(4);
            OffsetY = lump.ReadInt16(6);

            columns = new int[Width];
            for (int i = 0; i < Width; i++)
            {
                int index = lump.ReadInt32(8 + i * 4);

                columns[i] = index;
            }

            this.pallete = pallete;
            this.lump = lump.CopyRaw();
        }

        /// <summary>
        /// Returns an image copy of the graphic
        /// </summary>
        /// <returns></returns>
        public Bitmap ToImage()
        {
            
            Bitmap image = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            BitmapData locked = image.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, image.PixelFormat);

            IntPtr imagePointer = locked.Scan0;

            byte[] data = new byte[Math.Abs(locked.Stride) * image.Height];

            using (MemoryStream stream = new MemoryStream(lump)) {
                for (int i = 0; i < Width; i++)
                {
                    stream.Seek(columns[i], SeekOrigin.Begin);
                    int rowstart = 0;

                    while (rowstart != 255)
                    {
                        rowstart = stream.ReadByte();
                        if (rowstart == 0xFF) break;

                        int pixels = stream.ReadByte();
                        stream.ReadByte();

                        for (int j = 0; j < pixels; j++)
                        {
                            int pixel = stream.ReadByte();

                            RGB color = pallete[pixel];

                            data[4 * (i + (j + rowstart) * Width) + 0] = color.Blue;
                            data[4 * (i + (j + rowstart) * Width) + 1] = color.Green;
                            data[4 * (i + (j + rowstart) * Width) + 2] = color.Red;
                            data[4 * (i + (j + rowstart) * Width) + 3] = 255;
                        }

                        stream.ReadByte();
                    }
                }


                System.Runtime.InteropServices.Marshal.Copy(data, 0, imagePointer, data.Length);

                image.UnlockBits(locked);

                return image;
            }
        }

        /// <summary>
        /// Turns the image into a Rgba colormap
        /// </summary>
        /// <returns></returns>
        public RGBA[,] ToRgba2dArray()
        {
            using(MemoryStream stream = new MemoryStream(lump))
            {
                RGBA[,] array = new RGBA[Width, Height];

                for (int i = 0; i < columns.Length; i++)
                {
                    stream.Seek(columns[i], SeekOrigin.Begin);

                    int rowstart = 0;

                    while(rowstart != 255)
                    {
                        rowstart = stream.ReadByte();
                        if (rowstart == 255) break;

                        int length = stream.ReadByte();
                        stream.ReadByte();

                        for (int j = 0; j < length; j++)
                        {
                            array[i, rowstart + j] = pallete[stream.ReadByte()];
                        }

                        stream.ReadByte();
                    }
                }

                return array;
            }
        }
    }
}
