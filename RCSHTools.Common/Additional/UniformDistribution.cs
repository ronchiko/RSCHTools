namespace RCSHTools {
    public struct UniformDistribution
    {
        public float LowerBound {get;}
        public float UpperBound {get;}

        public static UniformDistribution Real => new UniformDistribution(0.0f, 1.0f);

        public UniformDistribution(float lowerBound, float upperBound){
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public float Distribute(float x){
            if(x >= LowerBound && x <= UpperBound)
                return 1 / (UpperBound - LowerBound);
            return 0;
        }
    }
}