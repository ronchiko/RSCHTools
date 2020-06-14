using static System.Math;

namespace RCSHTools {
    /// <summary>
    /// Provides gerenal utility functions
    /// </summary>
    public static class Utilities {
        /// <summary>
        /// Gets a bit by its index
        /// </summary>
        /// <param name="v"></param>
        /// <param name="bindex"></param>
        /// <returns></returns>
        public static int bit(this int v, int bindex){
            return (v & (1 << bindex)) == 0 ? 0 : 1;
        }
        /// <summary>
        /// <inheritdoc cref="bit(int, int)"/>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="bindex"></param>
        /// <returns></returns>
        public static int bit(this uint v, int bindex){
            return (v & (1 << bindex)) == 0 ? 0 : 1;
        }
        /// <summary>
        /// Rounds a floating point
        /// </summary>
        /// <param name="f"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public static float round(this float f, int post){
            int v = (int)(f * Pow(10, post));
            return v / (float)Pow(10, post);
        }

        /// <summary>
        /// Checks if a bit is active
        /// </summary>
        /// <param name="v"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool CheckBit(this int v , int index)
        {
            return bit(v, index) == 1;
        }
        /// <summary>
        /// Sets a bit in the array
        /// </summary>
        /// <param name="v"></param>
        /// <param name="index"></param>
        /// <param name="active"></param>
        public static void SetBit(this int v, int index, bool active)
        {
            _ = active ? v | (1 << index) : v & ~(1 << index);
        }
        /// <summary>
        /// Flips an integer value (Little Endian - Big Endian) 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int Flip(this int v)
        {
            int final = v;
            const int INT_SIZE = sizeof(int) - 1;
            for (int i = 0; i < sizeof(int) * 4; i++)
            {
                bool leftbit = final.CheckBit(INT_SIZE - i);

                final.SetBit(INT_SIZE - i, final.CheckBit(i));
                final.SetBit(i, leftbit);
            }
            return final;
        }
    }
}