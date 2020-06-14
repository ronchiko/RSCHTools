namespace RCSHTools{
    public struct Vector3{
        public float X {get; set;}
        public float Y {get; set;}
        public float Z {get; set;}

        public float Magnitude => (float)MathR.Sqrt(X * X + Y * Y + Z * Z);

        public Vector3(float x, float y, float z){
            X = x;
            Y = y;
            Z = z;
        }
        ///<summary>
        /// Returns the dot product of 2 vectors
        ///</summary>
        public static float Dot(Vector3 v1, Vector3 v2){
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        ///<summary>
        /// Returns the angle between 2 vector2 (in degrees)
        ///</summary>
        public static float Angle(Vector3 v1, Vector3 v2){
            return (float)System.Math.Acos(Dot(v1, v2)/(v1.Magnitude * v2.Magnitude)) * MathR.Rad2Deg;
        }
        ///<summary>
        /// Linearly interpolates between 2 vectors
        ///</summary>
        public static Vector3 Lerp(Vector3 a0, Vector3 a1, float t){
            return new Vector3(MathR.Lerp(a0.X, a1.X, t),
                MathR.Lerp(a0.Y, a1.Y, t),
                MathR.Lerp(a0.Z, a1.Z, t));
        }
    }
}