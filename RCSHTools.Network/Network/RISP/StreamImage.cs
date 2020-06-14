using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Network.RISP
{
    /// <summary>
    /// Represents a image that is sent over the network
    /// </summary>
    public class StreamImage
    {
        internal readonly int[] image;
        private readonly IndexCompressor environment;
        private readonly int width;
        private readonly int height;

        /// <summary>
        /// The width of the image
        /// </summary>
        public int Width => width;
        /// <summary>
        /// The height of the image
        /// </summary>
        public int Height => height;

        /// <summary>
        /// Creates a new receiver for the image
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public StreamImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            image = new int[width * height];
            environment = new IndexCompressor((uint)width, (uint)height, 0x01000000);
        }

        /// <summary>
        /// Returns the color of a pixel in the image using its coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public RGB GetPixel(int x, int y)
        {
            return new RGB(image[x + y * width]);
        }
        /// <summary>
        /// Sets a pixels color via its coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, RGB color)
        {
            image[x + y * width] = color.ToInt();
        }
        /// <summary>
        /// Sets a pixels color via its coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, int color)
        {
            image[x + y * width] = color;
        }
    }
}
