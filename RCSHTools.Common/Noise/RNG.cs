using static System.Math;
using System.Text;
using System;

namespace RCSHTools {
    ///<summary>
    /// An object that can generate random numbers
    ///</summary>
    public class RNG {
        
        private static int counter = 0;

        private const int RNG_SIZE = 64;

        private string binaryString;
        private int pointer;

        public int seed {get;}

        public static int Seed {
            get {
                var dt = DateTime.Now;
                counter++;
                return Abs((((int)Pow(dt.Millisecond,dt.Second) + dt.Day + dt.Millisecond - dt.Month) * counter + dt.Minute - dt.Hour * counter))% 9999;
            }
        }

        #region RNG Creator
        private static int GetLength(int x){
            return x * x + 2 * x + 7;
        }
        private static int h(int t){
            return (((t + 1) * (t * t) + 3)/ (3 * (t == 0 ? 1 : t) * ((int)Sqrt(t < 0 ? -t : t) + 1)));
        }
        #endregion


        public RNG() : this(Seed) {}
        /// <summary>
        /// Creates a new RNG object with a given seed
        /// </summary>
        /// <param name="seed">Seed of the object</param>
        public RNG(int seed){
            this.seed = Abs(seed) % 9999;
            int t = Abs(seed) % 9999;
            StringBuilder a = new StringBuilder();
            int length = GetLength(RNG_SIZE);
            for(int i = 0; i < length / 3;i++){
                t = h(t);
                a.Append(t.bit(0));
                a.Append(t.bit(1));
                a.Append(t.bit(2));

            }
            binaryString = a.ToString();
            pointer = 0;
        }
        /// <summary>
        /// Reads bytes from the binary string and transforms them into an integer
        /// </summary>
        /// <param name="start">Where to start reading from</param>
        /// <param name="amount">Amount of bits to read</param>
        /// <returns></returns>
        private uint Read(int start, int amount){
            int b = 0;
            for (int i = 0; i < amount; i++)
            {
                b = b << 1;
                b |= binaryString[(start + i) % binaryString.Length] == '0' ? 0 : 1;
            }
            return (uint)b;
        }
        /// <summary>
        /// Gets a random integer
        /// </summary>
        /// <returns></returns>
        public uint Get(){
            uint rnd = Read(pointer, sizeof(int) * 8);
            pointer += (int)((rnd * 3) % 7 + 1);
            pointer %= binaryString.Length;
            return rnd;
        }
        /// <summary>
        /// Gets a random integer between max
        /// </summary>
        /// <param name="max">Maximum value</param>
        /// <returns></returns>
        public uint Get(uint max){
            return Get() % max;
        }
        /// <summary>
        /// Gets a number between min and max
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns></returns>
        public int Get(int min, int max){
            return (Abs((int)Get()) % (max - min)) + min;
        }
        /// <summary>
        /// Gets a random float between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float GetF(){
            uint v1 = Get();
            uint v2 = Get();
            return (float)Min(v1, v2) / (Max(v1, v2) + 1.0f);
        }
        /// <summary>
        /// Gets a random float between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public float GetF(float min, float max)
        {
            return GetF() * (max - min) + min;
        }
    }
}