using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Represents a map texture
    /// </summary>
    public class MapTexture
    {
        private WadFile.WadTexturePool pool;
        private List<PatchData> patches;

        /// <summary>
        /// The nameof the texture
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Unused
        /// </summary>
        public int Masked { get; }
        /// <summary>
        /// The width of the texutre
        /// </summary>
        public short Width { get; }
        /// <summary>
        /// The height of the texture
        /// </summary>
        public short Height { get; }
        /// <summary>
        /// The amount of patches in the map
        /// </summary>
        public short Patches => (short)patches.Count;

        /// <summary>
        /// Creates a new map texture from a memeory stream
        /// </summary>
        /// <param name="texturePool"></param>
        /// <param name="stream"></param>
        public MapTexture(WadFile.WadTexturePool texturePool, MemoryStream stream)
        {
            pool = texturePool;
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            Name = DoomUtils.ToName(buffer, 0, 8);
            stream.Read(buffer, 0, 4);
            Masked = BitConverter.ToInt32(buffer, 0);
            stream.Read(buffer, 0, 2);
            Width = BitConverter.ToInt16(buffer, 0);
            stream.Read(buffer, 0, 2);
            Height = BitConverter.ToInt16(buffer, 0);

            stream.Read(buffer, 0, 4);

            this.patches = new List<PatchData>();
            stream.Read(buffer, 0, 2);
            short patches = BitConverter.ToInt16(buffer, 0);

            for (int i = 0; i < patches; i++)
            {
                this.patches.Add(new PatchData(stream));
            }
        }

        /// <summary>
        /// Gets a sub patch of this texture 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PatchData GetSubPatch(int index)
        {
            return patches[index];
        }

        /// <summary>
        /// Returns a bitmap of the colors outputed by the final wall texture 
        /// </summary>
        /// <returns></returns>
        public Bitmap GetBitmap()
        {
            Bitmap tex = new Bitmap(Width, Height);
            for (int i = 0; i < patches.Count; i++)
            {
                PatchData patch = patches[i];
                Bitmap image = pool[patches[i].PatchIndex];
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        if(patch.OriginX + x < tex.Width && patch.OriginY + y < tex.Height)
                            tex.SetPixel(patch.OriginX + x, patch.OriginY + y, image.GetPixel(x, y));
                    }
                }
                image.Dispose();
            }
            return tex;
        }
    }

    /// <summary>
    /// Represents a patch data under a <see cref="MapTexture"/>
    /// </summary>
    public struct PatchData
    {
        /// <summary>
        /// The X origin of the texture
        /// </summary>
        public short OriginX { get; }
        /// <summary>
        /// The Y origin of the texture
        /// </summary>
        public short OriginY { get; }
        /// <summary>
        /// The index of the patch in the <see cref="WadFile.WadTexturePool"/>
        /// </summary>
        public short PatchIndex { get; }
        /// <summary>
        /// 
        /// </summary>
        public short StepDir { get; }
        /// <summary>
        /// 
        /// </summary>
        public short ColorMap { get; }

        /// <summary>
        /// Creates a new patch data from a stream
        /// </summary>
        /// <param name="stream"></param>
        public PatchData(MemoryStream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            OriginX = BitConverter.ToInt16(buffer, 0);
            stream.Read(buffer, 0, 2);
            OriginY = BitConverter.ToInt16(buffer, 0);
            stream.Read(buffer, 0, 2);
            PatchIndex = BitConverter.ToInt16(buffer, 0);
            stream.Read(buffer, 0, 2);
            StepDir = BitConverter.ToInt16(buffer, 0);
            stream.Read(buffer, 0, 2);
            ColorMap = BitConverter.ToInt16(buffer, 0);
        }
    }
}
