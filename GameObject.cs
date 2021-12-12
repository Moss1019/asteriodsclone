
namespace WebSockServer
{
    internal abstract class GameObject
    {
        protected static readonly double MAX_X = 16.0;
        protected static readonly double MAX_Y = 12.0;

        private static int nextId = 0;

        public int Id { get; set; } = ++nextId;

        public Vector3 Position { get; set; } = new Vector3();

        public Vector3 Direction { get; set; } = new Vector3();

        public double Acceleration { get; set; } = 0.0;

        public abstract bool Update(double deltaTime);

        public static double DegToRad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public static double RadToDeg(double rad)
        {
            return rad * 180.0 / Math.PI;
        }
    }
}
