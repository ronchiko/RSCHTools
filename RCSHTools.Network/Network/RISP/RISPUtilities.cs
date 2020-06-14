using System;

namespace RCSHTools.Network.RISP
{
    /**
     * RISP - Ron's Image Streaming Protocol
     */

    /// <summary>
    /// Repsents a pixel in an image
    /// </summary>
    public struct RISPImagePixel : IIndexable
    {
        /// <summary>
        /// The x coordinate of the pixel
        /// </summary>
        public uint x;
        /// <summary>
        /// The y coordinate of the pixel
        /// </summary>
        public uint y;
        /// <summary>
        /// The color of the pixel
        /// </summary>
        public uint color;

        internal RISPImagePixel(uint x, uint y, uint color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }

        uint[] IIndexable.Read()
        {
            return new uint[] { x, y, color };
        }

        void IIndexable.Write(uint[] data)
        {
            x = data[0];
            y = data[1];
            color = data[2];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Pixel at (" + x + "," + y + ") changed to " + color.ToString("X");
        }
    }

    /// <summary>
    /// Represnets the nessery information for a <see cref="RISPServer"/> to allow pairing with a <see cref="RISPClient"/>. 
    /// The <see cref="RISPImageStructure"/> is valid if: 
    /// <list type="bullet">
    /// The data.Length == width * height</list>
    /// <list type="bullet">Width > 0</list>
    /// <list type="bullet">Height > 0</list>
    /// </summary>
    public struct RISPImageStructure
    {
        /// <summary>
        /// The width of the image
        /// </summary>
        public int width;
        /// <summary>
        /// The height of the image
        /// </summary>
        public int height;
        /// <summary>
        /// The color data of the image
        /// </summary>
        public int[] data;

        internal bool Validate => data != null && data.Length == width * height && width > 0 && height > 0;

        /// <summary>
        /// Creates a new <see cref="RISPImageStructure"/>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="data"></param>
        public RISPImageStructure(int width, int height, int[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
        }
    }

    /// <summary>
    /// Delegate for when the image update on the server
    /// </summary>
    /// <param name="changes"></param>
    /// <param name="sender"></param>
    public delegate void OnRISPImageUpdate(RISPImagePixel[] changes, object sender);
    /// <summary>
    /// Delegate for when a <see cref="RISPServer"/> initalizes an image
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="image"></param>
    /// <param name="sender"></param>
    public delegate void OnRISPImageInit(int width, int height, int[] image, object sender);
    /// <summary>
    /// Delegate for when the client is requested to send the source image
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public delegate RISPImageStructure OnRISPImageRequest(object sender);


    /// <summary>
    /// Occurs when a <see cref="RISPImageStructure"/> is invalid
    /// </summary>
    [Serializable]
    public class RispInvalidImageException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public RispInvalidImageException() { }
    }

    /// <summary>
    /// Occurs when a risp image dimensions are too large
    /// </summary>
    [Serializable]
    public class RispDimensionsTooLargeException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public RispDimensionsTooLargeException() { }
    }

    /// <summary>
    /// Occurs when a <see cref="RISPServer"/> recieves two diffrent dimensions from a client
    /// </summary>
    [Serializable]
    public class RispDimensionMismatchException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public RispDimensionMismatchException() { }
    }

    /// <summary>
    /// Occurs when a <see cref="RISPServer"/> declines a pairing from a <see cref="RISPClient"/>
    /// </summary>
    [Serializable]
    public class RispPairDeclinedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public RispPairDeclinedException() { }
    }
    /// <summary>
    /// Occurs when a client trys to send data to a server while the client is not paired to any server
    /// </summary>
    [Serializable]
    public class RispUnpairedClientException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public RispUnpairedClientException() { }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class RispTerminateThreadException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public RispTerminateThreadException() { }
    }

}
