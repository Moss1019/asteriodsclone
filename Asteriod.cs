
namespace WebSockServer
{
    internal class Asteriod : GameObject
    {
        private static double MAX_ACCELERATION = 5.0;

        public Asteriod()
        {
            Random rand = new Random();
            Direction = new Vector3(1, 0, 0);
            Acceleration = Math.Round(rand.NextDouble() * MAX_ACCELERATION, 2);
        }

        public override bool Update(double deltaTime)
        {
            var offset = deltaTime * Acceleration * Direction;
            Position += offset;
            if(Math.Abs(Position.X) > 17)
            {
                Position.X *= -1;
            }
            if(Math.Abs(Position.Y) > 17)
            {
                Position.Y *= -1;
            }
            return true;
        }
    }
}
