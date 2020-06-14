namespace RCSHTools {
    public struct Vector2 {
        public float X {get; set;}
        public float Y {get; set;}

        public float Magnitude => (float)MathR.Sqrt(X * X + Y * Y);

        public Vector2(float x, float y){
            X = x;
            Y = y;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t){
            return new Vector2(MathR.Lerp(a.X, b.X, t), MathR.Lerp(a.Y, b.Y, t));
        }
        ///<summary>
        /// Returns the angle between 2 vector2 (in degrees)
        ///</summary>
        public static float Angle(Vector2 v1, Vector2 v2){
            return (float)System.Math.Acos(Dot(v1, v2)/(v1.Magnitude * v2.Magnitude)) * MathR.Rad2Deg;
        }
        public static float Dot(Vector2 a, Vector2 b){
            return a.X * b.X + a.Y * b.Y;
        }
    
        public static implicit operator Vector2(Vector3 v3) => new Vector2(v3.X, v3.Y);
    }
}