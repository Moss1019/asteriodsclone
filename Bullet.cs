using System;

namespace WebSockServer
{
    internal class Bullet : GameObject
    {
        private static readonly double MAX_TIME_ALIVE = 4.0;

        public int OwnerId { get; set; } = 0;

        public double TimeAlive { get; set; } = 0.0;

        public Bullet()
        {
            Acceleration = 10.0;
        }

        public override bool Update(double deltaTime)
        {
            TimeAlive += deltaTime;
            if (TimeAlive > MAX_TIME_ALIVE)
            {
                return false;
            }
            Position += Acceleration * deltaTime * Direction;
            if (Math.Abs(Position.X) > MAX_X)
            {
                Position.X *= -1;
            }
            if (Math.Abs(Position.Y) > MAX_Y)
            {
                Position.Y *= -1;
            }
            return true;
        }
    }
}
