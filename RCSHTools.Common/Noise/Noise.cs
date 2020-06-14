using System;

namespace RCSHTools {
    ///<summary>
    /// Base class for 3d noise generation
    ///</summary>
    public class Noise3D {
        protected const int TABLE_SIZE = 256;
        protected const int TABLE_SIZE_MASK = TABLE_SIZE - 1;

        protected Vector3[] Gradient {get;}
        protected uint[] PremutationTable {get;}

        protected Noise3D(int seed){
            Gradient = new Vector3[TABLE_SIZE];
            PremutationTable = new uint[TABLE_SIZE * 2];
            RNG rng = new RNG(seed);
            
            // Create Gradient
            for (uint i = 0; i < TABLE_SIZE; i++)
            {
                Gradient[i] = new Vector3(rng.GetF(), rng.GetF(), rng.GetF());
                PremutationTable[i] = i;       
            }

            for (uint i = 0; i < TABLE_SIZE; i++)
            {
                int index = rng.Get(0, TABLE_SIZE);
                // Swap current index with the random index
                uint temp = PremutationTable[i];
                PremutationTable[i] = PremutationTable[index];
                PremutationTable[index] = PremutationTable[i];
            }

            for (int i = 0; i < TABLE_SIZE; i++)
            {
                PremutationTable[TABLE_SIZE + i] = PremutationTable[i]; 
            }
        }
    
        protected uint HashV3(int x, int y, int z){
            return PremutationTable[PremutationTable[PremutationTable[x] + y] + z];
        }
        protected uint HashV2(int x, int y){
            return PremutationTable[PremutationTable[PremutationTable[x] + y]];
        }
    }
}