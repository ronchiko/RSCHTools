
namespace RCSHTools
{
    /// <summary>
    /// Represents an RGB color
    /// </summary>
    public struct RGB
    {
        private const uint RED_CHANNEL = 0x000000ff, RED_SHIFT = 0;
        private const uint GREEN_CHANNEL = 0x0000ff00, GREEN_SHIFT = 8;
        private const uint BLUE_CHANNEL = 0x00ff0000, BLUE_SHIFT = 16;


        /// <summary>
        /// The red channel of the image
        /// </summary>
        public byte Red { get; set; }
        /// <summary>
        /// The green channel of the image
        /// </summary>
        public byte Green { get; set; }
        /// <summary>
        /// The blue channel of the image
        /// </summary>
        public byte Blue { get; set; }

        /// <summary>
        /// Creates an RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public RGB(byte r, byte g, byte b)
        {
            Red = r;
            Blue = b;
            Green = g;
        }
        /// <summary>
        /// Creates an RGB color from an compressed rgb
        /// </summary>
        /// <param name="c"></param>
        public RGB(int c)
        {
            Red = (byte)(c & RED_CHANNEL);
            Green = (byte)((c & GREEN_CHANNEL) >> (int)GREEN_SHIFT);
            Blue = (byte)((c & BLUE_CHANNEL) >> (int)BLUE_SHIFT);
        }

        /// <summary>
        /// Returns the color as a single integer value
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            return Red | (Green << (int)GREEN_SHIFT) | (Blue << (int)BLUE_SHIFT);
        }
        
        /// <summary>
        /// Returns a string of the color
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(RGB:" + Red + "," + Green + "," + Blue + ")";
        }
    }
    /// <summary>
    /// Represents an RGBA color
    /// </summary>
    public struct RGBA
    {
        private const uint RED_CHANNEL = 0x000000ff, RED_SHIFT = 0;
        private const uint GREEN_CHANNEL = 0x0000ff00, GREEN_SHIFT = 8;
        private const uint BLUE_CHANNEL = 0x00ff0000, BLUE_SHIFT = 16;
        private const uint ALPHA_CHANNEL = 0xff000000, ALPHASHIFT = 24;


        /// <summary>
        /// The red channel of the image
        /// </summary>
        public byte Red { get; set; }
        /// <summary>
        /// The green channel of the image
        /// </summary>
        public byte Green { get; set; }
        /// <summary>
        /// The blue channel of the image
        /// </summary>
        public byte Blue { get; set; }
        /// <summary>
        /// The alpha channel of the image
        /// </summary>
        public byte Alpha { get; set; }

        /// <summary>
        /// Creates an RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public RGBA(byte r, byte g, byte b, byte a = 255)
        {
            Red = r;
            Blue = b;
            Green = g;
            Alpha = a;
        }
        /// <summary>
        /// Creates an RGB color from an compressed rgb
        /// </summary>
        /// <param name="c"></param>
        public RGBA(int c)
        {
            Red = (byte)(c & RED_CHANNEL);
            Green = (byte)((c & GREEN_CHANNEL) >> (int)GREEN_SHIFT);
            Blue = (byte)((c & BLUE_CHANNEL) >> (int)BLUE_SHIFT);
            Alpha = (byte)((c & ALPHA_CHANNEL) >> (int)ALPHASHIFT);
        }

        /// <summary>
        /// Returns the color as a single integer value
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            return Red | (Green << (int)GREEN_SHIFT) | (Blue << (int)BLUE_SHIFT) | (Alpha << (int)ALPHASHIFT);
        }

        /// <summary>
        /// Returns a string of the color
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(RGBA:" + Red + "," + Green + "," + Blue + "," + Alpha + ")";
        }

        /// <summary>
        /// Creates a RGBA color from a RGB color
        /// </summary>
        /// <param name="c"></param>
        public static implicit operator RGBA(RGB c)
        {
            return new RGBA(c.Red, c.Green, c.Blue, 255);
        }
    }
}
