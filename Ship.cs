using System;

namespace WebSockServer
{
    internal class Ship : GameObject
    {
        private static readonly double ACCELERATION_PER_FRAME = 5;
        private static readonly double DAMPEN_PER_FRAME = 15;
        private static readonly double ACCELERATION_MAX = 8;
        private static readonly double ANGULAR_SPEED_PER_FRAME = 300;

        public bool Accelerating { get; set; } = false;

        public bool Rotating { get; set; } = false;

        public bool Thrust { get; set; } = false;

        public int RotateDir { get; set; } = 0;

        public int ThrustTimer { get; set; } = 0;

        public int Score { get; set; } = 0;

        public double Rotation { get; set; } = 0.0;

        public double AccelerationSpeed { get; set; } = 0.0;

        public Vector3 Trajectory { get; set; } = new Vector3();

        public override bool Update(double deltaTime)
        {
            var rotationDeg = RadToDeg(Rotation);
            if (Rotating)
            {
                rotationDeg += ANGULAR_SPEED_PER_FRAME * deltaTime * RotateDir;
            }
            if (rotationDeg < 0.0)
            {
                rotationDeg = 360.0;
            }
            else if (rotationDeg > 360.0)
            {
                rotationDeg = 0.0;
            }
            Rotation = DegToRad(rotationDeg);
            Direction = Vector3.CreateRotatedVector(Rotation);
            if (Accelerating)
            {
                ++ThrustTimer;
                AccelerationSpeed += ACCELERATION_PER_FRAME * deltaTime;
                if (AccelerationSpeed > ACCELERATION_MAX)
                {
                    AccelerationSpeed = ACCELERATION_MAX;
                }
                Trajectory = (Trajectory + .2 * Direction).Normalize();
                if(ThrustTimer % 4 < 2)
                { 
                    Thrust = true;
                }
                else
                {
                    Thrust = false;
                }
            }
            else
            {
                AccelerationSpeed -= DAMPEN_PER_FRAME * deltaTime;
                if (AccelerationSpeed < 0.0)
                {
                    AccelerationSpeed = 0.0;
                    Trajectory = new Vector3(Direction);
                }
                Thrust = false;
            }
            Position += AccelerationSpeed * deltaTime * Trajectory;
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
