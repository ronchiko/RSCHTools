namespace RCSHTools
{
    /// <summary>
    /// Provides simple mathematical operations
    /// </summary>
    public static class MathR
    {
        private static RNG random;
        private static Noises.PerlinNoise noise;

        /// <summary>
        /// PI
        /// </summary>
        public const float PI = 3.141592f;

        /// <summary>
        /// Converts Radians to Degress
        /// </summary>
        public const float Rad2Deg = PI/180f;
        /// <summary>
        /// Converts Degrees to Radians
        /// </summary>
        public const float Deg2Rad = 180f/PI;

        ///<summary>
        /// Raises a number by a power
        ///</summary>
        public static double Pow(double a, double b){
            if(b == 0)
                return 1;
            return a * Pow(a, b + (b > 0?-1:1));
        }
        ///<summary>
        /// Raises a number by a power
        ///</summary>
        public static float Pow(float a, float b) => (float)Pow((double)a, (double)b);

        ///<summary>
        /// Square root of a number
        ///</summary>
        public static float Sqrt(float n) => (float)Sqrt((double)n);
        ///<summary>
        /// Square root of a number
        ///</summary>
        public static double Sqrt(double n){
            if(n < 0){
                throw new System.Exception();
            }
            int v = 0;
            while(v * v <= n && (v + 1) * (v + 1) > n){
                v++;
            }
            return (float)SqrtRec(n, v != 0 ? v : v + 1, 2);
        }
        private static double SqrtRec(double n, double x,double p){
            double r = n / x;
            r = (r + x) / p;
            if(Abs(Pow(r, (int)p) - n) < .0000000001)
                return r;
            return SqrtRec(n, r, p);
        }
        ///<summary>
        /// The absulute value of a number
        ///</summary>
        public static double Abs(double d)
        {
            return d > 0 ? d : -d;
        }
        ///<summary>
        /// The absulute value of a number
        ///</summary>
        public static float Abs(float d)
        {
            return d > 0 ? d : -d;
        }
        ///<summary>
        /// Linearly interpolates between 2 values
        ///</summary>
        public static float Lerp(float v, float v1, float t){
            return (1 - t) * v + t * v1;
        }

        /// <summary>
        /// Clamps a value between 2 values
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float v, float min, float max){
            return v > max ? max : v < min ? min : v;
        }
        /// <summary>
        /// <inheritdoc cref="Clamp(float, float, float)"/>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(int v, int min, int max){
            return v > max ? max : v < min ? min : v;
        }

        /// <summary>
        /// Computes a smoothstep operation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="edge0"></param>
        /// <param name="edge1"></param>
        /// <returns></returns>
        public static float Smoothstep(float x, float edge0, float edge1){
            x = Clamp((x - edge0)/ (edge1 - edge0), 0, 1);
            return x * x * (3 - 2 * x);
        }

        /// <summary>
        /// Computes <see cref="Smoothstep(float, float, float)"/> between 0 and 1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Smoothstep(float x) => Smoothstep(x, 0f, 1f);

        ///<summary>
        /// Gets a float in the given range
        ///</summary>
        public static float RandomRange(float min, float max){
            if(random == null) random = new RNG();
            return random.GetF(min, max);
        }
        ///<summary>
        /// Gets an int in the given range
        ///</summary>
        public static int RandomRange(int min, int max){
            if(random == null) random = new RNG();
            return random.Get(min, max);
        }
        ///<summary>
        /// 3 Dimesional perlin noise
        ///</summary>
        public static float PerlinNoise(float x, float y, float z){
            if(noise == null) noise = new Noises.PerlinNoise();
            return noise.Evaluate(x, y, z);
        }
        ///<summary>
        /// 2 Dimesional perlin noise (z = 0)
        ///</summary>
        public static float PerlinNoise(float x, float y){
            return PerlinNoise(x, y, 0);
        }

        /// <summary>
        /// Rotates a long value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static long Rotate(long x, int l)
        {
            return (x >> l) & (x << (sizeof(long) - l)); 
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static long Maj(long x, long y, long z)
        {
            return (x ^ y) & (x ^ z) & (y ^ z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static long Ch(long x, long y, long z)
        {
            return (x ^ y) & (~x ^ z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static long Sigma0(long x)
        {
            return Rotate(x, 28) ^ Rotate(x, 34) ^ Rotate(x, 39);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static long Sigma1(long x)
        {
            return Rotate(x, 14) ^ Rotate(x, 18) ^ Rotate(x, 41);
        }


    }
}