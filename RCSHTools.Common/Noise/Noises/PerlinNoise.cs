using static System.Math;

namespace RCSHTools.Noises {
    public class PerlinNoise : Noise3D {
        public PerlinNoise(int seed) : base(seed){
        }
        public PerlinNoise() : base(RNG.Seed) {}

        public float Evaluate(float x, float y, float z){
            int xi0 = ((int)Floor(x)) & TABLE_SIZE_MASK;
            int yi0 = ((int)Floor(y)) & TABLE_SIZE_MASK;
            int zi0 = ((int)Floor(z)) & TABLE_SIZE_MASK;

            int xi1 = (xi0 + 1) & TABLE_SIZE_MASK;
            int yi1 = (yi0 + 1) & TABLE_SIZE_MASK;
            int zi1 = (zi0 + 1) & TABLE_SIZE_MASK;

            float tx = x - (int)Floor(x);
            float ty = y - (int)Floor(y);
            float tz = z - (int)Floor(z);

            float u = MathR.Smoothstep(tx);
            float v = MathR.Smoothstep(ty);
            float w = MathR.Smoothstep(tz);

            // Edge Vectors
            Vector3 c000 = Gradient[HashV3(xi0, yi0, zi0)];
            Vector3 c100 = Gradient[HashV3(xi1, yi0, zi0)];
            Vector3 c010 = Gradient[HashV3(xi0, yi1, zi0)];
            Vector3 c110 = Gradient[HashV3(xi1, yi1, zi0)];

            Vector3 c001 = Gradient[HashV3(xi0, yi0, zi1)];
            Vector3 c101 = Gradient[HashV3(xi1, yi0, zi1)];
            Vector3 c011 = Gradient[HashV3(xi0, yi1, zi1)];
            Vector3 c111 = Gradient[HashV3(xi1, yi1, zi1)];

            float x0 = tx, x1 = tx - 1;
            float y0 = ty, y1 = ty - 1;
            float z0 = tz, z1 = tz - 1;

            // Point Vectors
            Vector3 p000 = new Vector3(x0, y0, z0);
            Vector3 p100 = new Vector3(x1, y0, z0);
            Vector3 p010 = new Vector3(x0, y1, z0);
            Vector3 p110 = new Vector3(x1, y1, z0);

            Vector3 p001 = new Vector3(x0, y0, z1);
            Vector3 p101 = new Vector3(x1, y0, z1);
            Vector3 p011 = new Vector3(x0, y1, z1);
            Vector3 p111 = new Vector3(x1, y1, z1);

            // Lerping
            float a = MathR.Lerp(Vector3.Dot(c000, p000), Vector3.Dot(c100, p100), u);
            float b = MathR.Lerp(Vector3.Dot(c010, p010), Vector3.Dot(c110, p110), u);
            float c = MathR.Lerp(Vector3.Dot(c001, p001), Vector3.Dot(c101, p101), u);
            float d = MathR.Lerp(Vector3.Dot(c011, p011), Vector3.Dot(c111, p111), u);

            float e = MathR.Lerp(a, b, v);
            float f = MathR.Lerp(c, d, v);

            return MathR.Lerp(e, f, w);
        }

        public float Evaluate(float x, float y){
            return Evaluate(x, y, 0);
            /*int xi0 = ((int)Floor(x)) & TABLE_SIZE_MASK;
            int yi0 = ((int)Floor(y)) & TABLE_SIZE_MASK;

            int xi1 = (xi0 + 1) & TABLE_SIZE_MASK;
            int yi1 = (yi0 + 1) & TABLE_SIZE_MASK;

            float tx = x - (int)Floor(x);
            float ty = y - (int)Floor(y);

            float u = MathR.Smoothstep(tx);
            float v = MathR.Smoothstep(ty);

            // Corner Vectors
            Vector2 c00 = Gradient[HashV2(xi0, yi0)];
            Vector2 c10 = Gradient[HashV2(xi1, yi0)];
            Vector2 c01 = Gradient[HashV2(xi0, yi1)];
            Vector2 c11 = Gradient[HashV2(xi1, yi1)];

            float x0 = tx, x1 = tx - 1;
            float y0 = ty, y1 = ty - 1;

            // Point Vectors
            Vector2 p00 = new Vector2(x0, y0);
            Vector2 p10 = new Vector2(x1, y0);
            Vector2 p01 = new Vector2(x0, y1);
            Vector2 p11 = new Vector2(x1, y1);

            // Lerping
            float a = MathR.Lerp(Vector2.Dot(c00, p00), Vector2.Dot(c01, p01), u);
            float b = MathR.Lerp(Vector2.Dot(c10, p10), Vector2.Dot(c11, p11), u);

            return MathR.Lerp(a, b, v); */
        }
    }
}