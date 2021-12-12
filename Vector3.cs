
namespace WebSockServer
{
    internal class Vector3
    {
        public double X { get; set; } = 0.0;

        public double Y { get; set; } = 0.0;

        public double Z { get; set; } = 0.0;

        public Vector3() { }

        public Vector3(Vector3 vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
        }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double LengthSqr()
        {
            return X * X + Y * Y + Z * Z;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3 Direction(Vector3 other)
        {
            return this - other;
        }

        public double DistanceSqr(Vector3 other)
        {
            return Direction(other).LengthSqr();
        }

        public double Distance(Vector3 other)
        {
            return Direction(other).Length();
        }

        public Vector3 Normalize()
        {
            var length = Length();
            return new Vector3(X / length, Y / length, Z / length);
        }

        public Vector3 Rotate(Vector3 center, double radians)
        {
            var x = X - center.X;
            var y = Y - center.Y;
            var newX = x * Math.Cos(radians) - y * Math.Sin(radians);
            var newY = x * Math.Sin(radians) + y * Math.Cos(radians);
            newX += center.X;
            newY += center.Y;
            return new Vector3(newX, newY, 0);
        }

        public static Vector3 operator +(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
        }

        public static Vector3 operator -(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec2.X - vec1.X, vec2.Y - vec1.Y, vec2.Z - vec1.Z);
        }

        public static Vector3 operator *(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.Y * vec2.Z - vec1.Z * vec2.Y, vec1.X * vec2.Z - vec1.Z * vec2.X, vec1.X * vec2.Y - vec1.Y * vec2.X);
        }

        public static Vector3 operator *(double scaler, Vector3 vec)
        {
            return new Vector3(scaler * vec.X, scaler * vec.Y, scaler * vec.Z);
        }

        public static double Dot(Vector3 vec1, Vector3 vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        public static Vector3 CreateRotatedVector(double radians)
        {
            var center = new Vector3();
            var baseVec = new Vector3(0, 1, 0);
            var vec = baseVec.Rotate(center, radians);
            return vec;
        }
    }
}
