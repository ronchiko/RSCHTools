namespace RCSHTools.Noises {
    public class RidgedMultifractalNoise {
        private PerlinNoise noise;

        public RidgedMultifractalNoise(int seed){
            noise = new PerlinNoise(seed);
        }
        public RidgedMultifractalNoise(){
            noise = new PerlinNoise();
        }

        public float Evaluate(float x, float y){
            return 1-MathR.Abs(noise.Evaluate(x, y));
        }

        public float Evaluate(float x, float y, float z){
            return 1-MathR.Abs(noise.Evaluate(x, y, z));
        }
    }
}